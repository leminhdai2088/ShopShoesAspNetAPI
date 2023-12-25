using Payment.Domain.Entities;
using ShopShoesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.PaymentDestination
{
    public class PaymentDesService : IPaymentDes
    {
        private readonly MyDbContext context;
        public PaymentDesService(MyDbContext context) {
            this.context = context;
        }
        public async Task<string> Create(PaymentDesDto paymentDesDto)
        {
            try
            {
                var paymentDes = new PaymentDestinationEntity
                {
                    DesName = paymentDesDto.DesName,
                    DesShortName = paymentDesDto.DesShortName,
                    DesLogo = paymentDesDto.DesLogo,
                    ShortIndex = paymentDesDto.ShortIndex,
                    IsActive = paymentDesDto.IsActive,
                };
                await this.context.PaymentDesEntities.AddAsync(paymentDes);
                await this.context.SaveChangesAsync();
                return "Create payment destination successfully";
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
