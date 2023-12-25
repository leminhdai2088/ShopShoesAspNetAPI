using Payment.Application.Services.PaymentDestination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentNotification
{
    public interface IPaymentNoti
    {
        public Task<string> Create(PaymentNotiDto paymentNotiDto);
    }
}
