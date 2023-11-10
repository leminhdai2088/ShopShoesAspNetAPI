using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.common;
using System.Net;

namespace ShopShoesAPI.auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _iAuth;

        public AuthController(IAuth iAuth)
        {
            _iAuth = iAuth;
        }

        [HttpPost("login")]
        public async Task<ApiRespone> Login(LoginDTO user)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await _iAuth.Login(user)
            };
        }

        [HttpPost("register")]
        public async Task<ApiRespone> Register (RegisterDTO model)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.Created,
                Message = await _iAuth.Register(model)
            };
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult Test(string token)
        {
            // Lấy token từ header Authorization
            var authorizationHeader = Request.Headers.Authorization;
            return Ok(authorizationHeader);
        }
    }

    }

