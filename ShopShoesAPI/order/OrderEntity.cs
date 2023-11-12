using ShopShoesAPI.Enums;
using ShopShoesAPI.user;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopShoesAPI.order
{
    [Table("Orders")]
    public class OrderEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public string Note { get; set; }
        public PayMethod PayMethod { get; set; } = PayMethod.Momo;
        [Required, Phone]
        public string Phone { get; set; }

        public string UserId { get; set; }
        public UserEnityIndetity User { get; set; }

        public ICollection<OrderDetailEntity> OrderDetails { get; set; }

    }
}
