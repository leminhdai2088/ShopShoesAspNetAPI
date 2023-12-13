using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using ShopShoesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopShoesAPI.CheckoutServices
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
            var transaction = this.context.Database;
            try
            {
                await transaction.BeginTransactionAsync();
                var payment = await this.context.PaymentEntities
                    .FirstOrDefaultAsync(e => e.Id == paymentTransDto.PaymentId);

                if (payment == null)           
                    throw new Exception("Payment is not found");

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

                var sumTransAmount = this.context.PaymentTransEntities
                    .Where(e => e.PaymentId == paymentTransDto.PaymentId)
                    .Sum(e => e.TransAmount);

                payment.PaidAmount = sumTransAmount;
                payment.PaymentLastMessage = paymentTransDto.TransMessage ?? string.Empty;
                payment.PaymentStatus = paymentTransDto.TransStatus ?? payment.PaymentStatus;

                this.context.PaymentEntities.Update(payment);
                await this.context.SaveChangesAsync();
                await transaction.CommitTransactionAsync();
                return "Create payment transaction successfully";
            }
            catch (Exception ex)
            {
                await transaction.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }
            finally
            {
                await transaction.CloseConnectionAsync();
            }
        }
    }
}
