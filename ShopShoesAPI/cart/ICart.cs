using ShopShoesAPI.order;

namespace ShopShoesAPI.cart
{
    public interface ICart
    {
        public List<CartDTO> GetCartItems();
        public bool AddToCart(CartDTO newItem);
        public bool RemoveFromCart(int productId);
        public bool ClearCart();
        public decimal CalculateTotal();
        
    }
}
