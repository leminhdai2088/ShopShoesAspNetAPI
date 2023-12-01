using ShopShoesAPI.Enums;

namespace ShopShoesAPI.order
{
    public interface IOrder
    {
        public Task<OrderEntity> CheckoutAsync(string userId, OrderDTO orderDTO);
    }
}
