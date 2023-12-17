namespace ShopShoesAPI.CheckoutServices.Momo.Response
{
    public class MomoResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } = string.Empty;

        public void Set(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }
    }
}
