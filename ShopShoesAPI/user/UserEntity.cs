using ShopShoesAPI.common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopShoesAPI.enums;
namespace ShopShoesAPI.user
{
    [Table("users")]
    public class UserEntity : BaseEntity
    {

        [Key]
        public int Id { get; set; }

        [StringLength(100), Required]
        public string FullName { get; set; }

        [StringLength(100), Required]
        public string Email { get; set; }

        [MinLength(6), MaxLength(100), Required]
        public string Password { get; set; }

        [StringLength(10), Required]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public Roles Role { get; set; } = Roles.User;
    }
}
