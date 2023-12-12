namespace ShopShoesAPI.CheckoutServices
{
    public interface IPaymentNoti
    {
        public Task<string> Create(PaymentNotiDto paymentNotiDto);
    }
}
