
using Microsoft.OpenApi.Any;
using Payment.Domain.Entities;
using PaymentService.Vnpay.Response;
using ShopShoesAPI.CheckoutServices.Momo.Request;

namespace ShopShoesAPI.CheckoutServices
{
    public interface IPayment
    {
        public Task<object> Create(string userId, CreatePaymentDto paymentDto);
        public Task<PaymentEntity> FindById(int id);
        public Task<(PaymentReturnDto, string)> ProcessVnpayPaymentReturn(string userId, VnpayResponse request);
        public Task<VnpayPayIpnResponse> ProcessVnpayPaymentIpn(VnpayResponse request);
        public Task<(PaymentReturnDto, string)> ProcessMomoPaymentReturn(string userId, MomoOneTimePaymentResultRequest request);

    }
}
