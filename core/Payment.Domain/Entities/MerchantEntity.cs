using Payment.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("Merchant")]
    public class MerchantEntity : BaseAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string? MerchantName { get; set; } = string.Empty;
        public string? MerchantWeblink { get; set; } = string.Empty;
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        public string? MerchantReturnUrl { get; set; } = string.Empty;
        public string? SecretKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public List<PaymentNotificationEntity>? PaymentNotificationEntities { get; set; }
        public List<PaymentEntity>? PaymentEntities { get; set; }
    }
}
