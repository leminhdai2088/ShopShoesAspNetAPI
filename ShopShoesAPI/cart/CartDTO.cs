namespace ShopShoesAPI.cart
{
    public class CartDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
    }
}
