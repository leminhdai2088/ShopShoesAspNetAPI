using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.CheckoutServices;
using ShopShoesAPI.common;
using System.Net;

namespace ShopShoesAPI.OnlineCheckout
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayment payment;
        public PaymentController(IPayment payment)
        {
            this.payment = payment;
        }

        [HttpPost]
        public async Task<ApiRespone> Create(CreatePaymentDto paymentDto)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.Created,
                Message = "Create payment successfully",
                Metadata = await this.payment.Create(paymentDto)
            };
        }
    }
}
