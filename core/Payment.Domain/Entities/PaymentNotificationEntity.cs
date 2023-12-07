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
        public string Id { get; set; }
        public DateTime? NotiDate { get; set; }
        public string? NotiContent { get; set; } = string.Empty;
        public decimal? NotiAmount { get; set; }
        public string? NotiMessage { get; set; } = string.Empty;
        public string? NotiSignature { get; set; } = string.Empty;
        public string? NotiStatus { get; set; } = string.Empty;
        public DateTime? NotiResDate { get; set; }
        public string? NotiResMessage { get; set; } = string.Empty;
        public string? NotiResHttpCode { get; set; } = string.Empty;
        public string? PaymentRefId { get; set; } = string.Empty;

        public string? NotiPaymentId { get; set; } = string.Empty; //fk
        public PaymentEntity? PaymentEntity { get; set; }

        public string? NotiMerchantId { get; set; } = string.Empty; //fk
        public MerchantEntity? MerchantEntity { get; set; }


    }
}
