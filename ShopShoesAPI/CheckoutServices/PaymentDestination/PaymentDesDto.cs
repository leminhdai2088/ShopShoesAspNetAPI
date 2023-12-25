using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopShoesAPI.CheckoutServices
{
    public class PaymentDesDto
    {
        public string? DesName { get; set; } = string.Empty;
        public string? DesShortName { get; set; } = string.Empty;
        public string? DesLogo { get; set; } = string.Empty;
        public int ShortIndex { get; set; }
        public bool IsActive { get; set; } = true;
        public string? DesParentId { get; set; } = string.Empty; //fk
    }
}
