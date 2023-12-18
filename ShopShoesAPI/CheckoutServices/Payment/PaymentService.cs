using Azure.Messaging;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using Payment.Domain.Entities;
using PaymentService.Momo.Request;
using PaymentService.Vnpay.Config;
using PaymentService.Vnpay.Request;
using PaymentService.Vnpay.Response;
using ShopShoesAPI.cart;
using ShopShoesAPI.CheckoutServices.Momo.Config;
using ShopShoesAPI.CheckoutServices.Momo.Request;
using ShopShoesAPI.CheckoutServices.Momo.Response;
using ShopShoesAPI.Data;
using ShopShoesAPI.Enums;
using ShopShoesAPI.order;

namespace ShopShoesAPI.CheckoutServices
{
    public class PaymentServices : IPayment
    {
        private readonly MyDbContext context;
        private readonly VnpayConfig vnpayConfig;
        private readonly MomoConfig momoConfig;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string? IpAddress;
        private readonly IMerchant merchant;
        private readonly ICart cart;
        private readonly IOrder order;
        private static string Sphone;
        private static string Saddress;
        private static string Snote;
        private static PayMethod SpayMethod;
        private static OrderStatusEnum Sstatus;



        public PaymentServices(MyDbContext context, IOptions<VnpayConfig> vnpayConfigOption,
            IHttpContextAccessor httpContextAccessor, IMerchant merchant,
            IOptions<MomoConfig> momoConfigOption, ICart cart, IOrder order)
        {
            this.context = context;
            this.vnpayConfig = vnpayConfigOption.Value;
            this.httpContextAccessor = httpContextAccessor;
            this.IpAddress = this.httpContextAccessor?.HttpContext?.Connection?.LocalIpAddress?.ToString();
            this.merchant = merchant;
            this.momoConfig = momoConfigOption.Value;
            this.cart = cart;
            this.order = order;
        }   
        public async Task<object> Create(string userId, CreatePaymentDto paymentDto)
        {
            decimal RequiredAmount = this.cart.CalculateTotal();
            var transaction = this.context.Database;
            string? createMessage = string.Empty;
            Sphone = paymentDto.Phone ?? string.Empty;
            Saddress = paymentDto.Address ?? string.Empty;
            Snote = paymentDto.Note ?? string.Empty;
            SpayMethod = paymentDto.payMethod;

            try
            {
                await transaction.BeginTransactionAsync();

                if (paymentDto.payMethod == PayMethod.Cash || paymentDto.PaymentDesId == 0)
                {
                    var orderDTO = new OrderDTO
                    {
                        Phone = paymentDto.Phone,
                        Address = paymentDto.Address,
                        Note = paymentDto.Note,
                        payMethod = paymentDto.payMethod
                    };
                    var result = await this.order.CheckoutAsync(userId, orderDTO, null);
                    await transaction.CommitTransactionAsync();
                    await transaction.CloseConnectionAsync();
                    return result;
                }

                var payment = new PaymentEntity
                {
                    PaymentContent = paymentDto.PaymentContent, //
                    PaymentCurrency = paymentDto.PaymentCurrency ?? "vn", //
                    PaymentRefId = paymentDto.PaymentRefId, //
                    RequiredAmount = RequiredAmount, //
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
                await transaction.CommitTransactionAsync();
                await transaction.CloseConnectionAsync();

                var desEntity = await this.context.PaymentDesEntities
                    .FirstOrDefaultAsync(e => e.Id == paymentDto.PaymentDesId);
                    
                string paymentUrl = string.Empty;
                switch (desEntity?.DesShortName)
                {
                    case "VNPAY":
                        var vnpayPayRequest = new VnpayPayRequest(vnpayConfig.Version,
                                vnpayConfig.TmnCode, DateTime.Now, this.IpAddress ?? string.Empty,
                                RequiredAmount!, paymentDto.PaymentCurrency ?? string.Empty,
                                "other", paymentDto.PaymentContent ?? string.Empty, vnpayConfig.ReturnUrl, 
                                payment.Id.ToString() ?? string.Empty);

                        paymentUrl = vnpayPayRequest.GetLink(vnpayConfig.PaymentUrl, vnpayConfig.HashSecret);
                        break;
                    case "MOMO":
                        var momoOneTimePayRequest = new MomoOneTimePaymentRequest(momoConfig.PartnerCode,
                               payment.Id.ToString() ?? string.Empty, (long)RequiredAmount!, /*order id*/Guid.NewGuid().ToString() ?? string.Empty,
                                paymentDto.PaymentContent ?? string.Empty, momoConfig.ReturnUrl, momoConfig.IpnUrl, "captureWallet",
                                string.Empty);
                        momoOneTimePayRequest.MakeSignature(momoConfig.AccessKey, momoConfig.SecretKey);
                        (bool createMomoLinkResult, createMessage) = momoOneTimePayRequest.GetLink(momoConfig.PaymentUrl);
                        if (createMomoLinkResult)
                        {
                            paymentUrl = createMessage;
                        }
                        break;
                    default: 
                        break;
                }

                return new ResultPaymentLinksDto
                {
                    PaymentId = payment.Id,
                    PaymentUrl = paymentUrl,
                    Message = createMessage
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

        public async Task<(PaymentReturnDto, string)> ProcessVnpayPaymentReturn(string userId, VnpayResponse request)
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

                            var orderDTO = new OrderDTO
                            {
                                Address = Saddress,
                                Phone = Sphone,
                                Note = Snote,
                                payMethod = SpayMethod,
                                status = OrderStatusEnum.Completed
                            };
                            var savedOrder = await this.order.CheckoutAsync(userId, orderDTO, payment.Id.ToString());
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

        public async Task<VnpayPayIpnResponse> ProcessVnpayPaymentIpn(VnpayResponse request)
        {
            var resultData = new VnpayPayIpnResponse();
            try
            {
                var isValidSignature = request.IsValidSignature(this.vnpayConfig.HashSecret);
                if (isValidSignature)
                {
                    var payment = await FindById(Int32.Parse(request.vnp_TxnRef));
                    if (payment != null)
                    {
                        if(payment.RequiredAmount  == (request.vnp_Amount / 100))
                        {
                            if(payment.PaymentStatus != "0")
                            {
                                string message = "";
                                string status = "";

                                if(request.vnp_ResponseCode == "00" && request.vnp_TransactionStatus == "00")
                                {
                                    status = "0";
                                    message = "Trans success";
                                }
                                else
                                {
                                    status = "-1";
                                    message = "Trans error";
                                }

                                // update db
                                var paymentTrans = new PaymentTransactionEntity
                                {
                                    TransMessage = message,
                                    TransPayload = JsonConvert.SerializeObject(request),
                                    TransStatus = status,
                                    TransAmount = request.vnp_Amount,
                                    TransDate = DateTime.Now,
                                    PaymentId = Int32.Parse(request.vnp_TxnRef)
                                };
                                await this.context.PaymentTransEntities.AddAsync(paymentTrans);
                                var affectedRow = await this.context.SaveChangesAsync();

                                // confirm succress
                                Console.WriteLine(affectedRow);
                                if(affectedRow >= 1)
                                {
                                    resultData.Set("00", "Confirm success");
                                }
                            }
                            else
                            {
                                resultData.Set("02", "Order already confirmed");

                            }
                        }
                        else
                        {
                            resultData.Set("04", "Invalid amount");
                        }
                    }
                    else
                    {
                        resultData.Set("01", "Order not found");
                    }
                }
                else
                {
                    resultData.Set("97", "Invalid signature");

                }
            }
            catch (Exception ex)
            {
                ///TODO: prcess when exeption
                resultData.Set("99", "Input required data");
            }
            return resultData;
        }

        public async Task<(PaymentReturnDto, string)> ProcessMomoPaymentReturn(MomoOneTimePaymentResultRequest request)
        {
            string returnUrl = string.Empty;
            var resultData = new PaymentReturnDto();
            try
            {   
                var isValidSignature = request.IsValidSignature(
                    this.momoConfig.AccessKey, this.momoConfig.SecretKey);

                if (isValidSignature)
                {
                    var payment = await this.context.PaymentEntities
                        .FirstOrDefaultAsync(e => e.Id.ToString() == request.orderId);

                    if (payment != null)
                    {
                        var merchant = await this.context.MerchantEntities
                            .FirstOrDefaultAsync(e => e.Id == payment.MerchantId);
                        returnUrl = merchant?.MerchantReturnUrl ?? string.Empty;

                        if(request.resultCode == 0)
                        {
                            resultData.PaymentStatus = "00";
                            resultData.PaymentId = payment.Id;
                            resultData.Signature = Guid.NewGuid().ToString();
                            resultData.PaymentMessage = "Success";
                        }
                        else
                        {
                            resultData.PaymentStatus = "10";
                            resultData.PaymentMessage = "Payment process failed";
                        }
                    }
                    else
                    {
                        resultData.PaymentStatus = "11";
                        resultData.PaymentMessage = "Payment is not found";
                    }
                }
                else
                {
                    resultData.PaymentStatus = "99";
                    resultData.PaymentMessage = "Invalid sinature";

                }
            }
            catch (Exception ex)
            {
                resultData.PaymentStatus = "99";
                resultData.PaymentMessage = "Failed catched";
            }
            return (resultData, returnUrl);
        }

        private async Task<object> ProcessMomoPaymentIpn(MomoOneTimePaymentResultRequest request)
        {
            var result = new MomoResponse();
            try
            {
                var isValidSignature = request.IsValidSignature(momoConfig.AccessKey, momoConfig.SecretKey);

                if (isValidSignature)
                {
     
                    var payment = await this.context.PaymentEntities
                        .FirstOrDefaultAsync(e => e.Id.ToString() == request.orderId);

                    if (payment != null)
                    {
                        if (payment.RequiredAmount == request.amount)
                        {
                            if (payment.PaymentStatus != "0")
                            {
                                string message = "";
                                string status = "";

                                if (request.resultCode == 0)
                                {
                                    status = "0";
                                    message = "Tran success";
                                }
                                else
                                {
                                    status = "-1";
                                    message = "Tran error";
                                }

                                // update db
                                var paymentTrans = new PaymentTransactionEntity
                                {
                                    TransMessage = message,
                                    TransPayload = JsonConvert.SerializeObject(request),
                                    TransStatus = status,
                                    TransAmount = request.amount,
                                    TransDate = DateTime.Now,
                                    PaymentId = Int32.Parse(request?.orderId ?? string.Empty)
                                };
                                await this.context.PaymentTransEntities.AddAsync(paymentTrans);
                                var affectedRow = await this.context.SaveChangesAsync();

                                // confirm succress
                                Console.WriteLine(affectedRow);
                                if (affectedRow >= 1)
                                {
                                    result.Set(true, "Confirm success");
                                }
                                else
                                {
                                    result.Set(false, "Input required data");
                                }
                            }
                            else
                            {
                                result.Set(false, "Payment already confirmed");
                            }
                        }
                        else
                        {
                            result.Set(false, "Invalid amount");
                        }
                    }
                    else
                    {
                        result.Set(false, "Payment not found");
                    }
                }
                else
                {
                    result.Set(false, "Invalid signature");
                }
            }
            catch (Exception ex)
            {
                result.Set(false, "Error catch");
            }

            return Task.FromResult(result);
        }
    }
}
