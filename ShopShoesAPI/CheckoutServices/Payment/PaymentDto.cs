using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopShoesAPI.CheckoutServices
{
    public class CreatePaymentDto
    {
        public string? PaymentContent { get; set; } = string.Empty;
        public string? PaymentCurrency { get; set; } = string.Empty;
        public string? PaymentRefId { get; set; } = string.Empty;
        public decimal? RequiredAmount { get; set; }
        public string? PaymentLanguage { get; set; } = string.Empty;
        public string? Signature { get; set; } = string.Empty;
        
        public int MerchantId { get; set; } //fk
        public int PaymentDesId { get; set; } //fk

    }
    public class ResultPaymentLinksDto
    {
        public int? PaymentId { get; set; }
        public string? PaymentUrl { get; set; } = string.Empty;
    }
}
