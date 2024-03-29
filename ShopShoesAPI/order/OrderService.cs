﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using ShopShoesAPI.cart;
using ShopShoesAPI.Data;
using ShopShoesAPI.Enums;
using ShopShoesAPI.product;
using ShopShoesAPI.user;
using System.Text.Json.Serialization;
using System.Text.Json;
using ShopShoesAPI.common;

namespace ShopShoesAPI.order
{
    public class OrderService : IOrder
    {
        private readonly ICart iCart;
        private readonly IProduct product;
        private readonly MyDbContext context;
        private readonly UserManager<UserEnityIndetity> userManager;
        public OrderService(ICart iCart, UserManager<UserEnityIndetity> userManager, MyDbContext context, IProduct product)
        {
            this.iCart = iCart;
            this.userManager = userManager;
            this.context = context;
            this.product = product;
        }

        public async Task<bool> CheckoutAsync(string userId, OrderDTO orderDTO, string? paymentId = null)
        {
            try
            {
                var cartItems = this.iCart.GetCartItems(userId);

                
                if (cartItems == null)
                {
                    throw new Exception("Don't have item in the cart");
                }


                // Tạo đơn đặt hàng
                var order = new OrderEntity
                {
                    UserId = userId,
                    Phone = orderDTO.Phone ?? string.Empty,
                    Address = orderDTO.Address ?? string.Empty,
                    Note = orderDTO.Note ?? string.Empty,
                    PayMethod = orderDTO.payMethod,
                    Total = this.iCart.CalculateTotal(userId),
                    PaymentId = paymentId,
                    createdAt = DateTime.UtcNow
                };
                 // Lấy tổng giá tiền từ giỏ hàng
                // Thêm đơn đặt hàng vào cơ sở dữ liệu
                await this.context.AddAsync(order);
              
                await this.context.SaveChangesAsync();
                int orderId = order.Id;
                // Sau khi lưu đơn hàng, lấy order.Id mới được tạo
                foreach (object item in cartItems)
                {
                    object product = item.GetValObjDy("product");
                    var price = product.GetValObjDy("Price");
                    var productId = product.GetValObjDy("Id");

                    object cartItem = item.GetValObjDy("cart");
                    var quantity = cartItem.GetValObjDy("Qty");

                    var orderDetail = new OrderDetailEntity
                    {
                        Quantity = (int)quantity,
                        Total = (float)((float)price * (int)quantity),
                        ProductId = (int)productId,
                        OrderId = orderId,
                    };
                    await this.context.AddAsync(orderDetail);
                }
                await this.context.SaveChangesAsync();
                // Xóa giỏ hàng sau khi đã thanh toán
                await this.iCart.ClearCart(userId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<object> GetOrderByUserId(string userId)
        {
            try
            {
                var orders = from order in this.context.OrderEntities
                             join payment in this.context.PaymentEntities on order.PaymentId equals payment.Id.ToString()
                             join des in this.context.PaymentDesEntities on payment.PaymentDesId equals des.Id
                             orderby order.Id
                             select new
                             {
                                 Order = order,
                                 PaymentMessage = payment.PaymentLastMessage,
                                 PaymentContent = payment.PaymentContent,
                                 PaymentDes = des.DesShortName
                             };
                return orders;
                             
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<object> GetOrderDetails(int orderId)
        {
            var orderDetails = from orderDetail in this.context.OrderDetailEntities
                               join prod in this.context.ProductEntities on orderDetail.ProductId equals prod.Id
                               join order in this.context.OrderEntities on orderDetail.OrderId equals order.Id
                               join cate in this.context.CategoryEntities on prod.CategoryId equals cate.Id
                               join user in this.userManager.Users on order.UserId equals user.Id
                               where orderDetail.OrderId == orderId
                               orderby orderDetail.ProductId
                               select new
                               {
                                   OrderDetail = new
                                   {
                                       quantity  = orderDetail.Quantity,
                                       total = orderDetail.Total,
                                   },
                                   Product = new
                                   {
                                       name = prod.Name,
                                       price = prod.Price,
                                       desciption = prod.Description,
                                       quantity = prod.Quantity,
                                       discount = prod.Discount,
                                       image = prod.Image,
                                       rating = prod.Rating,
                                       categoryId = prod.CategoryId,
                                       categoryName = cate.Name
                                   },
                                   order,
                                   user = new
                                   {
                                       fullName = user.FullName,
                                       phone = user.PhoneNumber,
                                       address = user.Address,
                                       email = user.Email
                                   }
                               };
            return orderDetails.ToList();
        }

        public async Task<bool> HandleStatus(ChangeStatusDto changeStatus)
        {
            var transaction = this.context.Database;
            try
            {
                await transaction.BeginTransactionAsync();
                var order = await this.context.OrderEntities
                    .FirstOrDefaultAsync (e => e.Id == changeStatus.orderId);
                if(order == null)
                {
                    throw new Exception("Order is not found");
                }
                if(
                    (order.Status == OrderStatusEnum.Pending && (changeStatus.status == OrderStatusEnum.Confirmed || changeStatus.status == OrderStatusEnum.Cancelled)) ||
                    (order.Status == OrderStatusEnum.Shipped && changeStatus.status == OrderStatusEnum.Completed)
                    )
                {
                    order.Status = changeStatus.status;
                    this.context.OrderEntities.Update(order);
                    await this.context.SaveChangesAsync();
                    await transaction.CommitTransactionAsync();
                    return true;
                }
                else if (order.Status == OrderStatusEnum.Confirmed && (changeStatus.status == OrderStatusEnum.Shipped || changeStatus.status == OrderStatusEnum.Completed))
                {
                    var orderDetail = await this.context.OrderDetailEntities
                        .Where(e => e.OrderId == order.Id)
                        .ToArrayAsync();

                    foreach (var item in orderDetail)
                    {
                        var productId = item.ProductId;
                        var qty = item.Quantity;
                        await this.product.UpdateProductQty(productId, qty);
                    }
                    order.Status = changeStatus.status;
                    this.context.OrderEntities.Update(order);
                    await this.context.SaveChangesAsync();
                    await transaction.CommitTransactionAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }
            finally
            {
                await transaction.CloseConnectionAsync();
            }
        }
    }
}
