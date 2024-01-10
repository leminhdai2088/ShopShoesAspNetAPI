using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.auth;
using ShopShoesAPI.Enums;
using ShopShoesAPI.order;
using ShopShoesAPI.user;

namespace ShopShoesAPI.cart
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.User)]
    public class CartController : ControllerBase
    {
        private readonly ICart _cartService;
        private readonly IAuth _iAuth;
        private readonly PayloadTokenDTO payloadTokenDTO;
        private readonly string userId;
        public CartController(ICart cartService, IAuth iAuth, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _iAuth = iAuth;
            var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
            payloadTokenDTO = _iAuth.VerifyAccessToken(authorizationHeader!);
            userId = payloadTokenDTO?.Id;
        }
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var cartItems = _cartService.GetCartItems(userId);
            return Ok(cartItems);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(CartDTO newItem)
        {
            var result = await _cartService.AddToCart(newItem, userId);
            return Ok(result);
        }

        [HttpPost("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var result = await _cartService.RemoveFromCart(productId, userId);
            return Ok(result);
        }

        [HttpPost("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCart(userId);
            return Ok(result);
        }

        [HttpGet("total")]
        public  IActionResult CalculateTotal()
        {
            var total = _cartService.CalculateTotal(userId);
            return Ok(total);
        }
    }
}
