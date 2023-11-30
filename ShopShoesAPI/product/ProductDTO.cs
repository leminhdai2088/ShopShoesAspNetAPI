namespace ShopShoesAPI.product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public byte Discount { get; set; }
        public string Image { get; set; }
        public float Rating { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateProductDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public float? Price { get; set; }
        public int? Quantity { get; set; }
        public byte? Discount { get; set; }
        public string? Image { get; set; }
        public float? Rating { get; set; }
        public int? CategoryId { get; set; }
    }

    public class CreateProductDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public byte? Discount { get; set; }
        public string? Image { get; set; }
        public float? Rating { get; set; }
        public int CategoryId { get; set; }
    }
}