namespace ShopShoesAPI.cart
{
    public interface ICart
    {
        public Task<List<CartDTO>> GetCartItemsAsync(HttpContext httpContext);
        public Task<bool> AddToCartAsync(HttpContext httpContext, CartDTO newItem);
        public Task<bool> RemoveFromCartAsync(HttpContext httpContext, int productId);
        public Task<bool> ClearCartAsync(HttpContext httpContext);
        public Task<decimal> CalculateTotalAsync(HttpContext httpContext);
    }
}
