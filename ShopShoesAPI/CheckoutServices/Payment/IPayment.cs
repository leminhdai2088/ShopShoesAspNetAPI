
using Microsoft.OpenApi.Any;
using Payment.Domain.Entities;
using PaymentService.Vnpay.Response;
using ShopShoesAPI.CheckoutServices.Momo.Request;

namespace ShopShoesAPI.CheckoutServices
{
    public interface IPayment
    {
        public Task<object> Create(CreatePaymentDto paymentDto);
        public Task<PaymentEntity> FindById(int id);
        public Task<(PaymentReturnDto, string)> ProcessVnpayPaymentReturn(VnpayResponse request);
        public Task<VnpayPayIpnResponse> ProcessVnpayPaymentIpn(VnpayResponse request);
        public Task<(PaymentReturnDto, string)> ProcessMomoPaymentReturn(MomoOneTimePaymentResultRequest request);

    }
}
