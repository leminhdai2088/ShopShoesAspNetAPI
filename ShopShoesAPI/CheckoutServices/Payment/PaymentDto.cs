using ShopShoesAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopShoesAPI.CheckoutServices
{
    public class CreatePaymentDto
    {
        private string? paymentRefId = string.Empty;

        public string? PaymentContent { get; set; } = string.Empty;
        public string? PaymentCurrency { get; set; } = string.Empty;
        public string? PaymentRefId { get => paymentRefId; set => paymentRefId = value; }
        public string? PaymentLanguage { get; set; } = string.Empty;
        public string? Signature { get; set; } = string.Empty;


        public string? Phone { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public PayMethod payMethod { get; set; } = PayMethod.Cash;



        public int MerchantId { get; set; } //fk
        public int PaymentDesId { get; set; } //fk

    }
    public class ResultPaymentLinksDto
    {
        public int? PaymentId { get; set; }
        public string? PaymentUrl { get; set; } = string.Empty;
        public string? Message { get; set; } = string.Empty;
    }

    public class PaymentReturnDto
    {
        public int? PaymentId { get; set; }
        public string? PaymentStatus { get; set; } = string.Empty;
        public string? PaymentMessage { get; set; } = string.Empty;


        /// <summary>
        /// Format: yyyMMddHHmmss
        /// </summary>
        public string? PaymentDate { get; set; } = string.Empty;
        public string? PaymentRefId { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string? Signature { get; set; } = string.Empty;

    }
}
