using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopShoesAPI.CheckoutServices
{
    public interface IPaymentTrans
    {
        public Task<string> Create(PaymentTransDto paymentTransDto);
    }
}
