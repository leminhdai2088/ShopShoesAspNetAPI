using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Payment.Ultils.Extentions;
using PaymentService.Vnpay.Config;
using PaymentService.Vnpay.Response;
using ShopShoesAPI.CheckoutServices;
using ShopShoesAPI.CheckoutServices.Momo.Request;
using ShopShoesAPI.common;
using System.Net;

namespace ShopShoesAPI.OnlineCheckout
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayment payment;
        private readonly VnpayConfig vnpayConfig;

        public PaymentController(IPayment payment, IOptions<VnpayConfig> vnpayConfigOption)
        {
            this.payment = payment;
            this.vnpayConfig = vnpayConfigOption.Value;
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

        [HttpGet("{id}")]
        public async Task<ApiRespone> FindById(int id)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.Created,
                Message = string.Empty,
                Metadata = await this.payment.FindById(id)
            };
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnpayReturn([FromQuery] VnpayResponse response)
        {
            string returnUrl = string.Empty;
            var reuturnModel = new PaymentReturnDto();
            var processResult = await this.payment.ProcessVnpayPaymentReturn(response);

            if(!processResult.Item2.IsNullOrEmpty())
            {
                reuturnModel = processResult.Item1;
                returnUrl = processResult.Item2;
            }
            if (returnUrl.EndsWith("/"))
                returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);   
            return Redirect($"{returnUrl}?{reuturnModel.ToQueryString()}");
        }

        [HttpGet("momo-return")]
        public async Task<IActionResult> ProcessMomoPaymentReturn([FromQuery] MomoOneTimePaymentResultRequest response)
        {
            string returnUrl = string.Empty;
            var reuturnModel = new PaymentReturnDto();
            var processResult = await this.payment.ProcessMomoPaymentReturn(response);

            if (!processResult.Item2.IsNullOrEmpty())
            {
                reuturnModel = processResult.Item1;
                returnUrl = processResult.Item2;
            }
            if (returnUrl.EndsWith("/"))
                returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
            return Redirect($"{returnUrl}?{reuturnModel.ToQueryString()}");
        }

    }
}
