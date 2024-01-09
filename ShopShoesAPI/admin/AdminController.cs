using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.auth;
using ShopShoesAPI.common;
using ShopShoesAPI.enums;
using ShopShoesAPI.Enums;
using ShopShoesAPI.order;
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
        private readonly IOrder iOrder;
        public AdminController(IAdmin iAdmin, IAuth iAuth, IOrder iOrder)
        {
            this.iAdmin = iAdmin;
            this.iAuth = iAuth;
            this.iOrder = iOrder;
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

        [HttpGet("total-sale")]
        public async Task<ApiRespone> TotalSale()
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.iAdmin.CalculateTotalSale()
            };
        }

        [HttpGet("total-order")]
        public async Task<ApiRespone> TotalOrder()
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.iAdmin.CalculateTotalOrder()
            };
        }

        [HttpGet("total-product")]
        public async Task<ApiRespone> TotalProduct()
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.iAdmin.CalculateTotalProduct()
            };
        }

        [HttpGet("new-orders")]
        public async Task<ApiRespone> NewOrders()
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.iAdmin.CalculateTotalNewOrder()
            };
        }

        [HttpPatch("status")]
        public async Task<ApiRespone> HandleStatus([FromBody] ChangeStatusDto changeStatus)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Change status successfully",
                Metadata = await this.iOrder.HandleStatus(changeStatus)
            };
        }

        [HttpPost("get-orderDetail/{orderId}")]
        public ApiRespone GetOrderDetails([FromRoute] int orderId)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = this.iOrder.GetOrderDetails(orderId)
            };
        }

        [HttpPost("get-all-order")]
        public ApiRespone GetAllOrder()
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = this.iAdmin.GetAllOrders()
            };
        }

    }
}
