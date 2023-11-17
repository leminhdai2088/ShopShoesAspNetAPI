using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShopShoesAPI.common;
using ShopShoesAPI.Data;
using ShopShoesAPI.enums;
using ShopShoesAPI.user;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ShopShoesAPI.Enums;
using Microsoft.Extensions.DependencyInjection;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace ShopShoesAPI.auth
{
    public class AuthService : IAuth
    {
        private readonly MyDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly UserManager<UserEnityIndetity> userManager;
        private readonly SignInManager<UserEnityIndetity> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IDatabase redisDatabase;

        public AuthService(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor, UserManager<UserEnityIndetity> userManager,
            SignInManager<UserEnityIndetity> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConnectionMultiplexer redisConnectionMultiplexer
            )
        {
            this._context = context;
            this._appSettings = optionsMonitor.CurrentValue;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;

            this.redisDatabase = redisConnectionMultiplexer.GetDatabase();
        }





        public PayloadTokenDTO VerifyAccessToken(string accressToken)
        {
            string[] bearerToken = accressToken.Split(' ');
            string type = bearerToken[0];
            if(type != "Bearer")
            {
                throw new Exception("Invalid token type");
            }
            string token = bearerToken[1];

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(this._appSettings.SecretKey);

            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,

                ValidIssuer = this._appSettings.ValidIssuer,
                ValidAudience = this._appSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes)
            };
            try
            {
                // check token valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(token, tokenValidateParam, out var validatedToken);

                // check thuật toán
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        throw new BadHttpRequestException("Invalid token");
                    }
                }

                // check access token expire
                var utcExpireDate = long.Parse(tokenInVerification?.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var exprireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (exprireDate > DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("Token expired");
                }

                // Extract claims directly from validatedToken
                var idClaim = tokenInVerification.Claims.FirstOrDefault(c => c.Type == "Id");
                var emailClaim = tokenInVerification.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var roleClaim = tokenInVerification.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                // Access the values
                var idValue = idClaim.Value;
                var emailValue = emailClaim.Value;
                var roleValue = roleClaim.Value;

                PayloadTokenDTO payloadTokenDTO = new PayloadTokenDTO
                {
                    Id = idValue,
                    Email = emailValue,
                    Role = roleValue
                };
                return payloadTokenDTO;
            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                var emailExists = await this.userManager.FindByEmailAsync(registerDTO.Email);
                if (emailExists != null)
                {
                    throw new BadHttpRequestException("Email is already exists");
                }
                var phoneExists = await this.userManager.Users.SingleOrDefaultAsync(e => e.PhoneNumber == registerDTO.Phone);
                if (phoneExists != null) {
                    throw new BadHttpRequestException("Phone number is already exists");

                }
                var user = new UserEnityIndetity
                {
                    FullName = registerDTO.FullName,
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,
                    Address = registerDTO.Address,
                    PhoneNumber = registerDTO.Phone,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                if(await this.roleManager.RoleExistsAsync(Roles.User))
                {
                    var result =  await this.userManager.CreateAsync(user, registerDTO.Password);
                    if (!result.Succeeded)
                    {
                        throw new BadHttpRequestException("User failed to create");
                    }
                    // add role
                    await this.userManager.AddToRoleAsync(user, Roles.User);
                    return result;
                }
                else
                {
                    throw new BadHttpRequestException("Role is not found");
                }
            }
            catch(Exception ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
        }

        public async Task<TokenDTO> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                //var result = await this.signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false);
                var user = await this.userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    throw new BadHttpRequestException("Email is not registered");
                }
                else if (user!= null && (await this.userManager.CheckPasswordAsync(user, loginDTO.Password)) == false)
                {
                    throw new BadHttpRequestException("Invalid password");
                }
                else
                {
                    var accessToken = await GenerateTokenAsync(user, (int)ExprireTokenEnum.AccessToken);
                    var refreshToken = await GenerateTokenAsync(user, (int)ExprireTokenEnum.RefreshToken);

                    TimeSpan expiration = TimeSpan.FromDays((int)ExprireTokenEnum.RefreshToken);

                    bool resultSaveTokenToRedis = await this.redisDatabase.StringSetAsync(user.Id, refreshToken, expiration);
                    if (resultSaveTokenToRedis == false)
                    {
                        throw new Exception("Failed to save refresh token");
                    }
                    return new TokenDTO
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                    };
                }

            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
        }

        private async Task<string> GenerateTokenAsync(UserEnityIndetity user, int exprire)
        {
            var authClaims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var userRoles = await this.userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._appSettings.SecretKey));
            var token = new JwtSecurityToken(
                issuer: this._appSettings.ValidIssuer,
                audience: this._appSettings.ValidAudience,
                expires: DateTime.Now.AddDays(exprire),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        public async Task<string> LogoutAsync(string userId)
        {
            try
            {
                string token = await this?.redisDatabase.StringGetAsync(userId);
                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("You are not logged into the system yet");
                }
                bool resultDeltoken = await this.redisDatabase.KeyDeleteAsync(userId);
                if (resultDeltoken == false)
                {
                    throw new Exception("Cannot logout");
                }
                return "Log out";
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
