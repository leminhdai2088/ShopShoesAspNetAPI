using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Payment.Ultils.Extentions;
using PaymentService.Vnpay.Config;
using PaymentService.Vnpay.Response;
using ShopShoesAPI.auth;
using ShopShoesAPI.CheckoutServices;
using ShopShoesAPI.CheckoutServices.Momo.Request;
using ShopShoesAPI.common;
using ShopShoesAPI.Enums;
using ShopShoesAPI.user;
using System.Net;

namespace ShopShoesAPI.OnlineCheckout
{
    [Route("api/payment")]
    [ApiController]
    [Authorize(Roles = Roles.User)]
    public class PaymentController : ControllerBase
    {
        private readonly IPayment payment;
        private readonly VnpayConfig vnpayConfig;
        private readonly string userId;
        private readonly IAuth auth;
        private readonly PayloadTokenDTO payloadTokenDTO;
        public PaymentController(IPayment payment, IOptions<VnpayConfig> vnpayConfigOption,
            IHttpContextAccessor httpContextAccessor, IAuth iAuth)
        {
            var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
            this.payment = payment;
            this.vnpayConfig = vnpayConfigOption.Value;
            this.auth = iAuth;
            payloadTokenDTO = this.auth.VerifyAccessToken(authorizationHeader!);
            userId = payloadTokenDTO?.Id ?? string.Empty;
        }

        [HttpPost]
        public async Task<ApiRespone> Create(CreatePaymentDto paymentDto)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.Created,
                Message = "Create payment successfully",
                Metadata = await this.payment.Create(userId, paymentDto)
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
            var processResult = await this.payment.ProcessVnpayPaymentReturn(userId, response);

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
            var processResult = await this.payment.ProcessMomoPaymentReturn(userId,response);

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
