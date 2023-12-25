namespace ShopShoesAPI.CheckoutServices.ZaloPay.Config
{
    public class ZaloPayConfig
    {
        public static string ConfigName => "ZaloPay";
        public static string? AppUser {  get; set; } = string.Empty;
        public static string? PaymentUrl { get; set; } = string.Empty;
        public static string? RedirectUrl { get; set; } = string.Empty;
        public static string? InpUrl { get; set; } = string.Empty;
        public static string? AppId { get; set; } = string.Empty;
        public static string? Key1 { get; set; } = string.Empty;
        public static string? Key2 { get; set; } = string.Empty;



    }
}
