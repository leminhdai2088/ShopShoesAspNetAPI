using Payment.Application.Services.PaymentSignature;
using Payment.Domain.Entities;
using ShopShoesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentTransaction
{
    public class PaymentTransService: IPaymentTrans
    {
        private readonly MyDbContext context;

        public PaymentTransService(MyDbContext context)
        {
            this.context = context;
        }

        public async Task<string> Create(PaymentTransDto paymentTransDto)
        {
            try
            {
                var paymentTrans = new PaymentTransactionEntity
                {
                    TransMessage = paymentTransDto.TransMessage,
                    TransPayload = paymentTransDto.TransPayload,
                    TransStatus = paymentTransDto.TransStatus,
                    TransAmount = paymentTransDto.TransAmount,
                    TransDate = paymentTransDto.TransDate,
                    TransRefId = paymentTransDto.TransRefId,
                    PaymentId = paymentTransDto.PaymentId,
                };
                await this.context.PaymentTransEntities.AddAsync(paymentTrans);
                await this.context.SaveChangesAsync();
                return "Create payment transaction successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
