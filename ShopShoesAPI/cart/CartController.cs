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
            var cartItems = _cartService.GetCartItems();
            return Ok(cartItems);
        }

        [HttpPost("add")]
        public IActionResult AddToCart(CartDTO newItem)
        {
            var result = _cartService.AddToCart(newItem);
            return Ok(result);
        }

        [HttpPost("add-one")]
        public IActionResult AddOneItemToCart(int productId)
        {
            var result = _cartService.AddOneItemToCart(productId);
            return Ok(result);
        }

        [HttpPost("minus-one")]
        public IActionResult MinusOneItemToCart(int productId)
        {
            var result = _cartService.MinusOneItemToCart(productId);
            return Ok(result);
        }

        [HttpPost("remove/{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            var result = _cartService.RemoveFromCart(productId);
            return Ok(result);
        }

        [HttpPost("clear")]
        public IActionResult ClearCart()
        {
            var result =  _cartService.ClearCart();
            return Ok(result);
        }

        [HttpGet("total")]
        public  IActionResult CalculateTotal()
        {
            var total = _cartService.CalculateTotal();
            return Ok(total);
        }
    }
}
