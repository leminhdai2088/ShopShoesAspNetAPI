using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopShoesAPI.cart
{
    [Table("Cart")]   
    
    public class CartEntity
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public string UserId { get; set; }
    }
}
