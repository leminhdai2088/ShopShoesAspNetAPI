using Payment.Application.Services.Payment;

namespace ShopShoesAPI.CheckoutServices.Payment
{
    public interface IPayment
    {
        public Task<string> Create(CreatePaymentDto paymentDto);
    }
}
