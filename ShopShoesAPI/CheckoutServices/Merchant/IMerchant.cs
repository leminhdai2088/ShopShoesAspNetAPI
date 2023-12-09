using ShopShoesAPI.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services.Merchant
{
    public interface IMerchant
    {
        public Task<List<GetMerchantDto>> FindAll();
        public Task<GetMerchantDto> FindById(string id);
        public Task<string> Create(CreateMerchantDto merchant);
    }
}
