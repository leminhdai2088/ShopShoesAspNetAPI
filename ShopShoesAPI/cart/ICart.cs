using ShopShoesAPI.order;

namespace ShopShoesAPI.cart
{
    public interface ICart
    {
        public List<CartDTO> GetCartItems();
        public Task<bool> AddOneItemToCart(int productId);
        public bool MinusOneItemToCart(int productId);
        public Task<bool> AddToCart(CartDTO item);
        public bool RemoveFromCart(int productId);
        public bool ClearCart();
        public decimal CalculateTotal();
        
    }
}
