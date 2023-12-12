using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payment.Domain.Entities;
using PaymentService.Vnpay.Config;
using PaymentService.Vnpay.Request;
using ShopShoesAPI.Data;

namespace ShopShoesAPI.CheckoutServices
{
    public class PaymentServices : IPayment
    {
        private readonly MyDbContext context;
        private readonly VnpayConfig vnpayConfig;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string? IpAddress;

        public PaymentServices(MyDbContext context, IOptions<VnpayConfig> vnpayConfigOption,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.vnpayConfig = vnpayConfigOption.Value;
            this.httpContextAccessor = httpContextAccessor;
            this.IpAddress = this.httpContextAccessor?.HttpContext?.Connection?.LocalIpAddress?.ToString();
        }   
        public async Task<ResultPaymentLinksDto> Create(CreatePaymentDto paymentDto)
        {
            var transaction = this.context.Database;
            try
            {
               
                transaction.BeginTransaction();
                
                

                var payment = new PaymentEntity
                {
                    PaymentContent = paymentDto.PaymentContent, //
                    PaymentCurrency = paymentDto.PaymentCurrency, //
                    PaymentRefId = paymentDto.PaymentRefId, //
                    RequiredAmount = paymentDto.RequiredAmount, //
                    PaymentDate = DateTime.Now,
                    ExpireDate = DateTime.Now.AddMinutes(20),
                    PaymentLanguage = paymentDto.PaymentLanguage, //
                    MerchantId = paymentDto.MerchantId, //
                    PaymentDesId = paymentDto.PaymentDesId, // 
                };
                await this.context.AddAsync(payment);
                await this.context.SaveChangesAsync();

                var paymentSig = new PaymentSignatureEntity
                {
                    SignValue = paymentDto.Signature,
                    PaymentId = payment.Id,
                    SignOwn = paymentDto.MerchantId.ToString(),
                    Isvalid = true,
                };
                await this.context.AddAsync(paymentSig);
                await this.context.SaveChangesAsync();
                transaction.CommitTransaction();
                var desEntity = await this.context.PaymentDesEntities
                    .FirstOrDefaultAsync(e => e.Id == paymentDto.PaymentDesId);
                    

                
                string paymentUrl = string.Empty;
                switch (desEntity?.DesShortName)
                {
                    case "VNPAY":
                        var vnpayPayRequest = new VnpayPayRequest(vnpayConfig.Version,
                                vnpayConfig.TmnCode, DateTime.Now, this.IpAddress ?? string.Empty,
                                paymentDto.RequiredAmount ?? 0, paymentDto.PaymentCurrency ?? string.Empty,
                                "other", paymentDto.PaymentContent ?? string.Empty, vnpayConfig.ReturnUrl, 
                                payment.Id.ToString() ?? string.Empty);

                        paymentUrl = vnpayPayRequest.GetLink(vnpayConfig.PaymentUrl, vnpayConfig.HashSecret);

                        break;
                    default: 
                        break;
                }

                return new ResultPaymentLinksDto
                {
                    PaymentId = payment.Id,
                    PaymentUrl = paymentUrl,
                };
            }
            catch (Exception ex)
            {
                transaction.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }
    }
}
