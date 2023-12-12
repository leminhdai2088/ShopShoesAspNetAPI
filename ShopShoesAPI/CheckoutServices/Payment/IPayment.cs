
namespace ShopShoesAPI.CheckoutServices
{
    public interface IPayment
    {
        public Task<ResultPaymentLinksDto> Create(CreatePaymentDto paymentDto);
    }
}
