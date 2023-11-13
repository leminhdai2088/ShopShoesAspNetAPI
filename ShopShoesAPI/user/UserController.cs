using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.auth;
using ShopShoesAPI.common;
using ShopShoesAPI.email;
using ShopShoesAPI.Enums;
using System.Net;

namespace ShopShoesAPI.user
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.User)]
    public class UserController : ControllerBase
    {
        private readonly IUser _iUser;
        private readonly IAuth _iAuth;
        private readonly PayloadTokenDTO payloadTokenDTO;
        private readonly string userId;
        public UserController(IUser iUser, IAuth iAuth, IHttpContextAccessor httpContextAccessor)
        {
            _iUser = iUser;
            _iAuth = iAuth;
            var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
            payloadTokenDTO = _iAuth.VerifyAccessToken(authorizationHeader!);
            userId = payloadTokenDTO?.Id;
        }
        [HttpPost("profile")]
        public async Task<IActionResult> Profile()
        {
            return Ok(await _iUser.FindById(userId));
        }

        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] UpdateUserDTO updateUserDTO)
        {
            return Ok(await _iUser.Update(userId, updateUserDTO));
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            return Ok(await _iUser.ChangePassword(userId, changePasswordDTO));
        }

    }
}
