using ShopShoesAPI.comment;
using ShopShoesAPI.order;
using ShopShoesAPI.product;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ShopShoesAPI.product
{
    [Table("Products")]
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [AllowNull]
        public string Description { get; set; } = null!;

        [Range(0, float.MaxValue), Required]
        public float Price { get; set; }

        [Range(0, int.MaxValue), Required]
        public int Quantity { get; set; }

        public byte Discount { get; set; } = 0;

        public string Image { get; set; } = null!;
        [Range(0, 5)]
        public float Rating { get; set; } = 5!;


        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; }

        public ICollection<CommentEntity> Comments { get; set; }

        public ICollection<OrderDetailEntity> OrderDetails { get; set; }
    }
}
