﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.common;
using ShopShoesAPI.email;
using ShopShoesAPI.Enums;
using ShopShoesAPI.model;
using ShopShoesAPI.user;
using System.Net;

namespace ShopShoesAPI.auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _iAuth;
        private readonly IEmail _iEmail;

        public AuthController(IAuth iAuth, IEmail iEmail)
        {
            _iAuth = iAuth;
            _iEmail = iEmail;
        }

        [HttpPost("login")]
        public async Task<ApiRespone> Login([FromBody] LoginDTO user)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await _iAuth.LoginAsync(user)
            };
        }

        [HttpPost("register")]
        public async Task<ApiRespone> Register([FromBody] RegisterDTO model)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.Created,
                Message = "Register successfully",
                Metadata = await _iAuth.RegisterAsync(model)
            };
        }

        [HttpDelete("logout")]
        public async Task<ApiRespone> Logout([FromBody] string userId)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Message = await _iAuth.LogoutAsync(userId)
            }; 
        }

        [HttpPost("verify-accessToken")]
        public ApiRespone VerifyAccessToken([FromBody] string token)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Ok",
                Metadata = _iAuth.VerifyAccessToken(token)
            };
        }

        [HttpGet("testEmail")]
        public async Task<IActionResult> TestEmail(string token)
        {
            var message = 
                new MailMessage(new string[] { "20521153@gm.uit.edu.vn" }, "Test Subject", "Test");
            await _iEmail.SendEmail(message);
            return Ok();
   
        }
    }
}

