using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.auth;
using ShopShoesAPI.common;
using ShopShoesAPI.Enums;
using ShopShoesAPI.user;
using System.Net;

namespace ShopShoesAPI.order
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.User)]
    public class OrderController : ControllerBase
    {
        private readonly IOrder iOrder;
        private readonly PayloadTokenDTO payloadTokenDTO;
        private readonly string userId;
        private readonly IAuth _iAuth;
        public OrderController(IOrder iOrder, IAuth _iAuth, IHttpContextAccessor httpContextAccessor)
        {
            this.iOrder = iOrder;
            this._iAuth= _iAuth;
            var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
            payloadTokenDTO = this._iAuth.VerifyAccessToken(authorizationHeader!);
            userId = payloadTokenDTO?.Id;
        }

        [HttpPost("get")]
        public ApiRespone GetOrdersByUserId()
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = this.iOrder.GetOrderByUserId(userId)
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
    }

}
