using ShopShoesAPI.Enums;
using System.Collections;

namespace ShopShoesAPI.order
{
    public interface IOrder
    {
        public Task<bool> CheckoutAsync(string userId, OrderDTO orderDTO, string? paymentId);
        public Task<bool> HandleStatus(ChangeStatusDto changeStatus);
        public IEnumerable<object> GetOrderByUserId(string userId);
        public IEnumerable<object> GetOrderDetails(int orderId);
    }
}
