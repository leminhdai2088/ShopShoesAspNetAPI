
using Microsoft.OpenApi.Any;
using Payment.Domain.Entities;
using PaymentService.Vnpay.Response;

namespace ShopShoesAPI.CheckoutServices
{
    public interface IPayment
    {
        public Task<ResultPaymentLinksDto> Create(CreatePaymentDto paymentDto);
        public Task<PaymentEntity> FindById(int id);
        public Task<(PaymentReturnDto, string)> ProcessVnpayPaymentReturn(VnpayResponse request);
    }
}
