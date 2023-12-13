using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopShoesAPI.CheckoutServices
{
    public class PaymentTransDto
    {
        public string? TransMessage { get; set; } = string.Empty;
        public string? TransPayload { get; set; } = string.Empty;
        public string? TransStatus { get; set; } = string.Empty;
        public decimal? TransAmount { get; set; }
        public DateTime? TransDate { get; set; }
        public string? TransRefId { get; set; } = string.Empty;

        public int? PaymentId { get; set; } //fk
    }
}
