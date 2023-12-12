using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("PaymentNotification")]
    public class PaymentNotificationEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? NotiDate { get; set; } = DateTime.Now;
        public string? NotiContent { get; set; } = string.Empty;
        public decimal? NotiAmount { get; set; }
        public string? NotiMessage { get; set; } = string.Empty;
        public string? NotiSignature { get; set; } = string.Empty;
        public string? NotiStatus { get; set; } = string.Empty;
        public DateTime? NotiResDate { get; set; }
        public string? NotiResMessage { get; set; } = string.Empty;
        public string? NotiResHttpCode { get; set; } = string.Empty;
        public string? PaymentRefId { get; set; } = string.Empty;

        public int? NotiPaymentId { get; set; } //fk
        public PaymentEntity? PaymentEntity { get; set; }

        public int? NotiMerchantId { get; set; } //fk
        public MerchantEntity? MerchantEntity { get; set; }


    }
}
