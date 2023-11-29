using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShopShoesAPI.user;
using ShopShoesAPI.product;

namespace ShopShoesAPI.comment
{
    [Table("Comments")]
    public class CommentEntity
    {
        [Key]
        public int Id { get; set; }
        [Range(0, 5)]
        public float Rating { get; set; }
        public string Content { get; set; } = null!;

        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }


    }
}
