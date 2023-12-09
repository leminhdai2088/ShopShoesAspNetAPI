using NetTopologySuite.Operation.Valid;
using Payment.Domain.Entities;
using ShopShoesAPI.CheckoutServices.Payment;
using ShopShoesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Payment.Application.Services.Payment
{
    public class PaymentService : IPayment
    {
        private readonly MyDbContext context;

        public PaymentService(MyDbContext context)
        {
            this.context = context;
        }
        public async Task<string> Create(CreatePaymentDto paymentDto)
        {
            var transaction = this.context.Database;
            try
            {
               
                transaction.BeginTransaction();
                paymentDto.PaymentDate = DateTime.Now;
                paymentDto.ExpireDate = DateTime.Now.AddMinutes(10);

                var payment = new PaymentEntity
                {
                    PaymentContent = paymentDto.PaymentContent,
                    PaymentCurrency = paymentDto.PaymentCurrency,
                    PaymentRefId = paymentDto.PaymentRefId,
                    RequiredAmount = paymentDto.RequiredAmount,
                    PaymentDate = paymentDto.PaymentDate,
                    ExpireDate = paymentDto.ExpireDate,
                    PaymentLanguage = paymentDto.PaymentLanguage,
                    PaidAmount = paymentDto.PaidAmount,
                    PaymentStatus = paymentDto.PaymentStatus,
                    PaymentLastMessage = paymentDto.PaymentLastMessage,
                    MerchantId = paymentDto.MerchantId,
                };
                await this.context.AddAsync(payment);

                var paymentSig = new PaymentSignatureEntity
                {
                    SignValue = "hihi",
                    SignAlgo = "Hmash256",
                    SignOwn = "hihi",
                    SignDate = DateTime.Now,
                    Isvalid = true,
                    PaymentId = payment.Id
                };
                await this.context.AddAsync(paymentSig);
                await this.context.SaveChangesAsync();
                transaction.CommitTransaction();
                return "Create payment successfully!!!";
            }
            catch (Exception ex)
            {
                transaction.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }
    }
}
