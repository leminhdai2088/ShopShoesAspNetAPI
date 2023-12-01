using Microsoft.AspNetCore.Identity;
using ShopShoesAPI.cart;
using ShopShoesAPI.Data;
using ShopShoesAPI.Enums;
using ShopShoesAPI.user;

namespace ShopShoesAPI.order
{
    public class OrderService : IOrder
    {
        private readonly ICart iCart;
        private readonly MyDbContext context;
        private readonly UserManager<UserEnityIndetity> userManager;
        public OrderService(ICart iCart, UserManager<UserEnityIndetity> userManager, MyDbContext context)
        {
            this.iCart = iCart;
            this.userManager = userManager;
            this.context = context;
    
        }

        public async Task<OrderEntity> CheckoutAsync(string userId, OrderDTO orderDTO)
        {
            try
            {
                var cartItems = this.iCart.GetCartItems();
                if (cartItems == null)
                {
                    throw new Exception("Don't have item in the cart");
                }

                var user = await this.userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("User is not found");
                }

                // Tạo đơn đặt hàng
                var order = new OrderEntity
                {
                    UserId = userId,
                    Phone = orderDTO.Phone ?? user.PhoneNumber,
                    Address = orderDTO.Address ?? user.Address,
                    Note = orderDTO.Note ?? "",
                    PayMethod = orderDTO.payMethod
                };
                order.Total = this.iCart.CalculateTotal(); // Lấy tổng giá tiền từ giỏ hàng
                // Thêm đơn đặt hàng vào cơ sở dữ liệu
                await this.context.AddAsync(order);
              
                await this.context.SaveChangesAsync();
                int orderId = order.Id;
                // Sau khi lưu đơn hàng, lấy order.Id mới được tạo
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetailEntity
                    {
                        Quantity = item.Quantity,
                        Total = item.Price * item.Quantity,
                        ProductId = item.ProductId,
                        OrderId = orderId,
                    };

                    await this.context.AddAsync(orderDetail);

                }
                await this.context.SaveChangesAsync();


                // Xóa giỏ hàng sau khi đã thanh toán
                this.iCart.ClearCart();

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
