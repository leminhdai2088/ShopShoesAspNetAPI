namespace ShopShoesAPI.CheckoutServices.ZaloPay.Response
{
    public class CreateZalopayPayResponse
    {
        public int? returnCode { get; set; } 
        public string? returnMessage { get; set; } = string.Empty;
        public string orderUrl { get; set; } = string.Empty;
    }
}
