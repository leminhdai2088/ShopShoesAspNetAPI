using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.Merchant
{
    public class CreateMerchantDto
    {
        public string? MerchantName { get; set; } = string.Empty;
        public string? MerchantWeblink { get; set; } = string.Empty;
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        public string? MerchantReturnUrl { get; set; } = string.Empty;
        public string? SecretKey { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;
    }

    public class GetMerchantDto
    {
        public string? MerchantName { get; set; } = string.Empty;
        public string? MerchantWeblink { get; set; } = string.Empty;
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        public string? MerchantReturnUrl { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;
    }
}
