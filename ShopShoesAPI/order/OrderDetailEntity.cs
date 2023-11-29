using ShopShoesAPI.product;
using ShopShoesAPI.user;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopShoesAPI.order
{
    [Table("OrderDetails")]
    public class OrderDetailEntity
    {
        [Required, Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Range(0, float.MaxValue)]
        public float Total { get; set; }

        public int OrderId { get; set; }
        public OrderEntity Order { get; set; }

        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }
    }
}
