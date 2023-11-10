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

namespace ShopShoesAPI.auth
{
    public class AuthService : IAuth
    {
        private readonly MyDbContext _context;
        private readonly AppSettings _appSettings;
        
        public AuthService(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }



        public async Task<TokenDTO> Login(LoginDTO model)
        {
            var user = await _context.UserEntity.SingleOrDefaultAsync(e => e.Email == model.Email);
            if (user == null)
            {
                throw new BadHttpRequestException("Email is not registered!");
            }
            string passwordHashed = user.Password;
            bool verified = BCrypt.Net.BCrypt.Verify(model.Password, passwordHashed);

            if (verified == false)
            {
                throw new BadHttpRequestException("Invalid password!");
            }


            var payload = new PayloadTokenDTO
            {
                Id = user.Id,
                Role = user.Role
            };
            return new TokenDTO
            {
                AccessToken = await GenerateToken(payload, 1),
                RefreshToken = await GenerateToken(payload, 7)
            };
        }

        public async Task<string> GenerateToken(PayloadTokenDTO payload, int exprire)
        {
            if(exprire <= 0 ) {
                throw new BadHttpRequestException("Invaid expire value");
            }
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", payload.Id.ToString()),
                    new Claim("Role", payload.Role.ToString()),
                    //new Claim("TokenId", Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(exprire),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKeyBytes), 
                    SecurityAlgorithms.HmacSha512Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDesc);

            return await Task.FromResult(jwtTokenHandler.WriteToken(token));
        }

      

        public async Task<string> Register(RegisterDTO model)
        {
            var isExistsUser = await _context.UserEntity.SingleOrDefaultAsync(e => e.Email == model.Email);
            if(isExistsUser != null)
            {
                throw new BadHttpRequestException("Email is already registered!");
            }
            isExistsUser = await _context.UserEntity.SingleOrDefaultAsync(e => e.Phone == model.Phone);
            if (isExistsUser != null)
            {
                throw new BadHttpRequestException("Phone number is already registered!");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var userEntity = new UserEntity
            {
                FullName = model.FullName,
                Email = model.Email,
                Address = model.Address,
                Password  = passwordHash,
                Phone = model.Phone,
            };
            await _context.UserEntity.AddAsync(userEntity);
            
            await _context.SaveChangesAsync();
            return "Register successfully!";
        }

        public async Task<PayloadTokenDTO> VerifyAccessToken(string accressToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenValidateParam = new TokenValidationParameters
            {
                // Tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                // Ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false // ko kiểm tra hết hạn
            };
            try
            {
                // check token valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(accressToken, tokenValidateParam, out var validatedToken);

                // check thuật toán
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        throw new BadHttpRequestException("Invalid token");
                    }
                }

                // check access token expire
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var exprireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if(exprireDate > DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("Token expired");
                }

                // Extract claims directly from validatedToken
                var idClaim = tokenInVerification.Claims.FirstOrDefault(c => c.Type == "Id");
                var roleClaim = tokenInVerification.Claims.FirstOrDefault(c => c.Type == "Role");

                if (idClaim == null || roleClaim == null)
                {
                    throw new BadHttpRequestException("Id or Role claim not found in the token");
                }

                // Access the values
                var idValue = idClaim.Value;
                var roleValue = roleClaim.Value;

                return new PayloadTokenDTO
                {
                    Id = int.Parse(idValue),
                    Role = roleValue == "User" ? Roles.User : Roles.Admin
                };
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
    }
}
