using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.CheckoutServices;
using ShopShoesAPI.common;
using System.Net;

namespace ShopShoesAPI.OnlineCheckout
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchant merchant;

        public MerchantController(IMerchant merchant) {
            this.merchant = merchant;
        }

        [HttpGet("{id}")]
        public async Task<ApiRespone> FindById(int id)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.merchant.FindById(id),
            }; 
        }

        [HttpGet]
        public async Task<ApiRespone> FindAll(string id)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.merchant.FindAll(),
            };
        }

        [HttpPost]
        public async Task<ApiRespone> Create(CreateMerchantDto merchant)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.Created,
                Message = await this.merchant.Create(merchant),
            };
        }

    }
}
