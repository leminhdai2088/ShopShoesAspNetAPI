using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using ShopShoesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.Merchant
{
    public class MerchantService : IMerchant
    {
        private readonly MyDbContext context;

        public MerchantService(MyDbContext context) {
            this.context = context;
        }
        public async Task<string> Create(CreateMerchantDto merchant)
        {
            try
            {
                var newMerChant = new MerchantEntity
                {
                    MerchantName = merchant.MerchantName,
                    MerchantWeblink = merchant.MerchantWeblink,
                    MerchantIpnUrl = merchant.MerchantIpnUrl,
                    MerchantReturnUrl = merchant.MerchantReturnUrl,
                    SecretKey = merchant.SecretKey,
                    IsActive = true
                };
                await this.context.AddAsync(newMerChant);
                await this.context.SaveChangesAsync();
                return "Create merchant successfully!";
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetMerchantDto> FindById(string id)
        {
            try
            {
                var merchant = await this.context.MerchantEntities.FirstOrDefaultAsync(e => e.Id == id);
                var result = new GetMerchantDto
                {
                    MerchantName = merchant.MerchantName ?? "",
                    MerchantWeblink = merchant.MerchantWeblink ?? "",
                    MerchantIpnUrl = merchant.MerchantIpnUrl ?? "",
                    MerchantReturnUrl = merchant.MerchantReturnUrl ?? "",
                    IsActive = merchant.IsActive
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetMerchantDto>> FindAll()
        {
            try
            {
                var merchantEntities = await this.context.MerchantEntities.ToListAsync();
                List<GetMerchantDto> result = new List<GetMerchantDto>();
                merchantEntities.ForEach((e) =>
                {
                    var merchantDto = new GetMerchantDto
                    {
                        MerchantName = e.MerchantName ?? "",
                        MerchantWeblink = e.MerchantWeblink ?? "",
                        MerchantIpnUrl = e.MerchantIpnUrl ?? "",
                        MerchantReturnUrl = e.MerchantReturnUrl ?? "",
                        IsActive = e.IsActive
                    };
                    result.Add(merchantDto);
                });
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
