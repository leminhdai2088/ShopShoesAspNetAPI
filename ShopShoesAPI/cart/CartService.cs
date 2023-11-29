using Microsoft.AspNetCore.Identity;
using ShopShoesAPI.Data;
using ShopShoesAPI.order;
using ShopShoesAPI.user;
using System.Text.Json;

namespace ShopShoesAPI.cart
{
    public class CartService : ICart
    {
        private readonly MyDbContext _context;
        private readonly UserManager<UserEnityIndetity> userManager;
        public CartService(MyDbContext context, UserManager<UserEnityIndetity> userManager)
        {
            this._context = context;
            this.userManager = userManager;
        }
        public async Task<List<CartDTO>> GetCartItemsAsync(HttpContext httpContext)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();
            return cartItems;
        }

        public async Task<bool> AddToCartAsync(HttpContext httpContext, CartDTO newItem)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == newItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                cartItems.Add(newItem);
            }

            httpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(HttpContext httpContext, int productId)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                httpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
                return true;
            }
            return false;
        }

        public async Task<bool> ClearCartAsync(HttpContext httpContext)
        {
            httpContext.Session.Remove("Cart");
            return true;
        }

        public async Task<decimal> CalculateTotalAsync(HttpContext httpContext)
        {
            var cart = httpContext.Session.GetString("Cart");
            var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

            decimal total = (decimal)cartItems.Sum(item => item.Price * item.Quantity);
            return total;
        }

        public async Task<OrderEntity> CheckoutAsync(HttpContext httpContext, string userId, OrderDTO orderDTO)
        {
            try
            {
                var cartItems = await GetCartItemsAsync(httpContext);
                if(cartItems == null)
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
                    Note = orderDTO.Note ?? ""
                };
                order.Total = await CalculateTotalAsync(httpContext); // Lấy tổng giá tiền từ giỏ hàng
                // Thêm đơn đặt hàng vào cơ sở dữ liệu
                this._context.Add(order);
                await this._context.SaveChangesAsync();

                // Sau khi lưu đơn hàng, lấy order.Id mới được tạo
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetailEntity
                    {
                        Quantity = item.Quantity,
                        Total = item.Price * item.Quantity,
                        ProductId = item.ProductId,
                        OrderId = order.Id
                    };

                    this._context.Add(orderDetail);
                    
                }
                await this._context.SaveChangesAsync();


                // Xóa giỏ hàng sau khi đã thanh toán
                await ClearCartAsync(httpContext);

                return order;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
