using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Vnpay.Response
{
    public class VnpayPayIpnResponse
    {
        public string? RspCode {  get; set; }  = string.Empty;
        public string? Message { get; set; } = string.Empty;

        public VnpayPayIpnResponse() { }
        public VnpayPayIpnResponse(string rspCode, string message)
        { 
            this.RspCode = rspCode;
            this.Message = message;
        }

        public void Set(string rspCode, string message)
        {
            this.RspCode = rspCode;
            this.Message = message;
        }
    }
}
