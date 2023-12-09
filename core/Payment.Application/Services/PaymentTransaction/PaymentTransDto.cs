using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentTransaction
{
    public class PaymentTransDto
    {
        public string? TransMessage { get; set; } = string.Empty;
        public string? TransPayload { get; set; } = string.Empty;
        public string? TransStatus { get; set; } = string.Empty;
        public DateTime? TransAmount { get; set; }
        public DateTime? TransDate { get; set; }
        public string? TransRefId { get; set; } = string.Empty;

        public string? PaymentId { get; set; } = string.Empty; //fk
    }
}
