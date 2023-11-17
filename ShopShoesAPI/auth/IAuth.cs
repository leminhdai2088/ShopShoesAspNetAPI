using Microsoft.AspNetCore.Identity;
using ShopShoesAPI.common;
using ShopShoesAPI.user;

namespace ShopShoesAPI.auth
{
    public interface IAuth
    {
        //Task<TokenDTO> Login(LoginDTO model);
        //Task<string> GenerateToken(PayloadTokenDTO payload, int exprire);
        //Task<string> Register(RegisterDTO model);
        PayloadTokenDTO VerifyAccessToken(string accressToken);

        ///////////////////////////
        public Task<IdentityResult> RegisterAsync(RegisterDTO registerDTO);
        public Task<TokenDTO> LoginAsync(LoginDTO loginDTO);
        public Task<string> LogoutAsync(string userId);
    }
}
