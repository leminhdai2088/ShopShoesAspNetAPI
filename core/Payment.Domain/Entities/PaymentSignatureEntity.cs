using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("PaymentSignature")]
    public class PaymentSignatureEntity
    {
        [Key]
        public string Id { get; set; }
        public string? SignValue { get; set; } = string.Empty;
        public string? SignAlgo { get; set; } = string.Empty;
        public string? SignOwn { get; set; } = string.Empty;
        public DateTime SignDate { get; set; }
        public bool Isvalid { get; set; }

        public string? PaymentId { get; set; } = string.Empty; //fk
        public PaymentEntity? PaymentEntity { get; set; }
    }
}
