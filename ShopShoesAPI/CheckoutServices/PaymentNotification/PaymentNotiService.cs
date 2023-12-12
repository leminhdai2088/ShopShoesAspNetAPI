using Payment.Domain.Entities;
using ShopShoesAPI.Data;

namespace ShopShoesAPI.CheckoutServices
{
    public class PaymentNotiService: IPaymentNoti
    {
        private readonly MyDbContext context;

        public PaymentNotiService(MyDbContext context) {
            this.context = context;
        }

        public async Task<string> Create(PaymentNotiDto paymentNotiDto)
        {
            try
            {
                var paymentNoti = new PaymentNotificationEntity
                {
                    NotiDate = paymentNotiDto.NotiDate,
                    NotiContent = paymentNotiDto.NotiContent,
                    NotiAmount = paymentNotiDto.NotiAmount,
                    NotiMessage = paymentNotiDto.NotiMessage,
                    NotiSignature = paymentNotiDto.NotiSignature,
                    NotiStatus = paymentNotiDto.NotiStatus,
                    NotiResDate = paymentNotiDto.NotiResDate,
                    NotiResMessage = paymentNotiDto.NotiResMessage,
                    NotiResHttpCode = paymentNotiDto.NotiResHttpCode,
                    PaymentRefId = paymentNotiDto.PaymentRefId,

                    NotiPaymentId = paymentNotiDto.NotiPaymentId,
                    NotiMerchantId = paymentNotiDto.NotiMerchantId,
                };
                await this.context.PaymentNotiEntities.AddAsync(paymentNoti);
                await this.context.SaveChangesAsync();
                return "Create payment notification successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
