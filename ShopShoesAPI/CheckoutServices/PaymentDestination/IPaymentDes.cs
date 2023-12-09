using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentDestination
{
    public interface IPaymentDes
    {
        public Task<string> Create(PaymentDesDto paymentDesDto);
    }
}
