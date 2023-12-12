using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentSignature
{
    public class PaymentSigDto
    {
        public string? SignValue { get; set; } = string.Empty;
        public string? SignAlgo { get; set; } = string.Empty;
        public string? SignOwn { get; set; } = string.Empty;
        public DateTime SignDate { get; set; }
        public bool Isvalid { get; set; }

        public int? PaymentId { get; set; } //fk
    }
}
