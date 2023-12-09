using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentSignature
{
    public interface IPaymentSig
    {
        public Task<string> Create(PaymentSigDto paymentSigDto);
    }
}
