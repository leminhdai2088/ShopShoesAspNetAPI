using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.auth;
using ShopShoesAPI.common;
using ShopShoesAPI.enums;
using ShopShoesAPI.Enums;
using System.Net;

namespace ShopShoesAPI.admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin iAdmin;
        private readonly IAuth iAuth;
        public AdminController(IAdmin iAdmin, IAuth iAuth)
        {
            this.iAdmin = iAdmin;
            this.iAuth = iAuth;
        }

        [HttpGet("users")]
        public async Task<ApiRespone> FindAllUser([FromQuery] QueryAndPaginateDTO queryAndPaginate,
            [FromQuery] StatusUserEnum? statusUserEnum = StatusUserEnum.All)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.iAdmin.FindAllUser(queryAndPaginate, statusUserEnum)
            };
        }

        [HttpPost("user")]
        public async Task<ApiRespone> CreateUser([FromBody] RegisterDTO registerDTO)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.Created,
                Message = "Register successfully",
                Metadata = await iAuth.RegisterAsync(registerDTO)
            };
        }

        [HttpGet("orders")]
        public async Task<ApiRespone> FinnAllOrder([FromQuery] QueryAndPaginateDTO queryAndPaginate, 
            [FromQuery] OrderStatusEnum? status)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.iAdmin.FindAllOrder(queryAndPaginate, status)
            };
        }

        [HttpDelete("user/{userId}")]
        public async Task<ApiRespone> DeletUserById(string userId)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Delete successfully",
                Metadata = await iAdmin.DeleteUserById(userId)
            };
        }

    }
}
