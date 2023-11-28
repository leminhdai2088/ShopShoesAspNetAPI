using System.Text.Json;

namespace ShopShoesAPI.cart
{
    public class CartService : ICart
    {
        public async Task<List<CartDTO>> GetCartItemsAsync(HttpContext httpContext)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();
            return cartItems;
        }

        public async Task<bool> AddToCartAsync(HttpContext httpContext, CartDTO newItem)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == newItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                cartItems.Add(newItem);
            }

            httpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(HttpContext httpContext, int productId)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                httpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
                return true;
            }
            return false;
        }

        public async Task<bool> ClearCartAsync(HttpContext httpContext)
        {
            httpContext.Session.Remove("Cart");
            return true;
        }

        public async Task<decimal> CalculateTotalAsync(HttpContext httpContext)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

            decimal total = (decimal)cartItems.Sum(item => item.Price * item.Quantity);
            return total;
        }
    }
}
