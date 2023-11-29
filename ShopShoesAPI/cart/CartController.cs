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
        public async Task<IActionResult> GetCartItems()
        {
            var cartItems = await _cartService.GetCartItemsAsync(HttpContext);
            return Ok(cartItems);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(CartDTO newItem)
        {
            var result = await _cartService.AddToCartAsync(HttpContext, newItem);
            return Ok(result);
        }

        [HttpPost("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var result = await _cartService.RemoveFromCartAsync(HttpContext, productId);
            return Ok(result);
        }

        [HttpPost("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync(HttpContext);
            return Ok(result);
        }

        [HttpGet("total")]
        public async Task<IActionResult> CalculateTotal()
        {
            var total = await _cartService.CalculateTotalAsync(HttpContext);
            return Ok(total);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderDTO orderDTO)
        {
            try
            {
                var order = await _cartService.CheckoutAsync(HttpContext, userId, orderDTO);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
