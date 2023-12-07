using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("Payment")]
    public class PaymentEntity
    {
        [Key]
        public string Id { get; set; }
        public string? PaymentContent { get; set; } = string.Empty;
        public string? PaymentCurrency { get; set; } = string.Empty;
        public string? PaymentRefId { get; set; } = string.Empty;
        public decimal? RequiredAmount { get; set; } 
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public DateTime? ExpireDate { get; set; } 
        public string? PaymentLanguage { get; set; } = string.Empty;
        public decimal? PaidAmount { get; set; } 
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentLastMessage { get; set; } = string.Empty;

        public string MerchantId { get; set; } = string.Empty; //fk
        public MerchantEntity? MerchantEntity { get; set; }

        public string PaymentDesId { get; set; } = string.Empty; //fk
        public PaymentDestinationEntity? PaymentDestinationEntity { get; set; }

        public List<PaymentNotificationEntity>? PaymentNotificationEntities { get; set; }
        public List<PaymentTransactionEntity>? PaymentTransactionEntities { get; set; }
        public List<PaymentSignatureEntity>? PaymentSignatureEntities { get; set; }
    }
}
