using ShopShoesAPI.order;

namespace ShopShoesAPI.cart
{
    public interface ICart
    {
        public IEnumerable<object> GetCartItems(string userId);
        public Task<bool> AddToCart(CartDTO item, string userId);
        public Task<bool> RemoveFromCart(int productId, string userId);
        public Task<bool> ClearCart(string userId);
        public decimal CalculateTotal(string userId);
        
    }
}
