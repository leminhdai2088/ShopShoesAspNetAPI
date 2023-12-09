using NetTopologySuite.Operation.Valid;
using Payment.Application.Services.PaymentDestination;
using Payment.Domain.Entities;
using ShopShoesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentSignature
{
    public class PaymentSigService : IPaymentSig
    {
        private readonly MyDbContext context;

        public PaymentSigService(MyDbContext context)
        {
            this.context = context;
        }

        public async Task<string> Create(PaymentSigDto paymentSigDto)
        {
            try
            {
                var paymentSig = new PaymentSignatureEntity
                {
                    SignValue = paymentSigDto.SignValue,
                    SignAlgo = paymentSigDto.SignAlgo,
                    SignOwn = paymentSigDto.SignOwn,
                    SignDate = paymentSigDto.SignDate,
                    Isvalid = paymentSigDto.Isvalid,
                    PaymentId = paymentSigDto.PaymentId,
                };
                await this.context.PaymentSigEntities.AddAsync(paymentSig);
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
