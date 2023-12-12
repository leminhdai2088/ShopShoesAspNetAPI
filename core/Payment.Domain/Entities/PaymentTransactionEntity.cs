using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("PaymentTransaction")]
    public class PaymentTransactionEntity
    {
        [Key]
        public int Id { get; set; }
        public string? TransMessage { get; set; } = string.Empty;
        public string? TransPayload { get; set; } = string.Empty;
        public string? TransStatus { get; set; } = string.Empty;
        public DateTime? TransAmount { get; set; }
        public DateTime? TransDate { get; set; }
        public string? TransRefId { get; set; } = string.Empty;

        public int? PaymentId { get; set; } //fk
        public PaymentEntity? PaymentEntity { get; set; }
    }
}
