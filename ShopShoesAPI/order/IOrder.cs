using ShopShoesAPI.Enums;

namespace ShopShoesAPI.order
{
    public interface IOrder
    {
        public Task<bool> CheckoutAsync(string userId, OrderDTO orderDTO, string? paymentId);
        public Task<bool> HandleStatus(int orderId, OrderStatusEnum status);
    }
}
