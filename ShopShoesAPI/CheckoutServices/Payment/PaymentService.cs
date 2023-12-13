using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Payment.Domain.Entities;
using PaymentService.Vnpay.Config;
using PaymentService.Vnpay.Request;
using PaymentService.Vnpay.Response;
using ShopShoesAPI.Data;

namespace ShopShoesAPI.CheckoutServices
{
    public class PaymentServices : IPayment
    {
        private readonly MyDbContext context;
        private readonly VnpayConfig vnpayConfig;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string? IpAddress;
        private readonly IMerchant merchant;

        public PaymentServices(MyDbContext context, IOptions<VnpayConfig> vnpayConfigOption,
            IHttpContextAccessor httpContextAccessor, IMerchant merchant)
        {
            this.context = context;
            this.vnpayConfig = vnpayConfigOption.Value;
            this.httpContextAccessor = httpContextAccessor;
            this.IpAddress = this.httpContextAccessor?.HttpContext?.Connection?.LocalIpAddress?.ToString();
            this.merchant = merchant;
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

        public async Task<PaymentEntity> FindById(int id)
        {
            try
            {
                var payment = await this.context?.PaymentEntities?.FirstOrDefaultAsync(e => e.Id == id);
                if (payment == null)
                    throw new Exception("Payment is not found");
                return payment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(PaymentReturnDto, string)> ProcessVnpayPaymentReturn(VnpayResponse request)
        {
            try
            {
                string returnUrl = string.Empty;
                var resultData = new PaymentReturnDto();
                var isValidSignature = request.IsValidSignature(vnpayConfig.HashSecret);
                /*
                respone code: 
                    00: success
                    99: invalid
                    10: failed
                */
                if (isValidSignature)
                {
                    if (request.vnp_ResponseCode == "00")
                    {
                        var payment = await FindById(Int32.Parse(request.vnp_TxnRef));
                        if (payment != null)
                        {
                            var merchant = await this.merchant.FindById(payment.MerchantId);

                            returnUrl = merchant?.MerchantReturnUrl ?? string.Empty;

                            resultData.PaymentStatus = "00";
                            resultData.PaymentId = payment.Id;
                            ///TODO: make signature
                            resultData.Signature = Guid.NewGuid().ToString();
                        }
                        else
                        {
                            resultData.PaymentStatus = "11";
                            resultData.PaymentMessage = "Can't find payment";
                        }
                    }
                    else
                    {
                        resultData.PaymentStatus = "10";
                        resultData.PaymentMessage = "Payment proceess failed";
                    }
                }
                else
                {
                    resultData.PaymentStatus = "99";
                    resultData.PaymentMessage = "Payment proceess failed";
                }
                return (resultData, returnUrl);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
