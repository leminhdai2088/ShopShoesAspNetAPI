using ShopShoesAPI.common;
using ShopShoesAPI.Enums;
using ShopShoesAPI.user;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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
        public string Note { get; set; } = String.Empty;
        public PayMethod PayMethod { get; set; } = PayMethod.Cash;
        [Required, Phone]
        public string Phone { get; set; }

        public string UserId { get; set; }

        [AllowNull]
        public string? PaymentId { get; set; } = string.Empty;


        [Required]
        public decimal Total { get; set; }

        public UserEnityIndetity User { get; set; }

        public ICollection<OrderDetailEntity> OrderDetails { get; set; }

    }
}
