using ShopShoesAPI.common;
using ShopShoesAPI.user;

namespace ShopShoesAPI.auth
{
    public interface IAuth
    {
       Task<TokenDTO> Login(LoginDTO model);
       Task<string> GenerateToken(PayloadTokenDTO payload, int exprire);
       Task<string> Register(RegisterDTO model);
       Task<PayloadTokenDTO> VerifyAccessToken(string accressToken);

    }
}
