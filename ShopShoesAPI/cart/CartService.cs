using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using ShopShoesAPI.Data;
using ShopShoesAPI.order;
using ShopShoesAPI.product;
using ShopShoesAPI.user;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopShoesAPI.cart
{
    public class CartService : ICart
    {
        private readonly MyDbContext _context;
        private readonly IProduct product;
        public CartService(MyDbContext context, IProduct product)
        {
            this._context = context;
            this.product = product;
        }
        public object[] GetCartItems(string userId)
        {
            try
            {
                var res = from cart in this._context.CartEntities
                          join product in this._context.ProductEntities on cart.ProductId equals product.Id
                          where cart.UserId == userId
                          select new
                          {
                                  cart,
                                  product
                          };
                return res.ToArray();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddToCart(CartDTO item, string userId)
        {
            try
            {
                bool isValidQty = await this.product.IsValidBuyQty(item.ProductId, item.Quantity);
                if (!isValidQty)
                {
                    return isValidQty;
                }
                var cart = this._context.CartEntities
                    .FirstOrDefault(e => (e.ProductId == item.ProductId && e.UserId == userId));
                if (cart != null)
                {
                    cart.Qty = item.Quantity;
                    this._context.CartEntities.Update(cart);
                }
                else
                {
                    var newItem = new CartEntity
                    {
                        ProductId = item.ProductId,
                        Qty = item.Quantity,
                        UserId = userId
                    };
                    this._context.CartEntities.Add(newItem);
                }
                await this._context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
               return false;
            }
        }

        public async Task<bool> RemoveFromCart(int productId, string userId)
        {
            try
            {
                var cart = this._context.CartEntities
                    .FirstOrDefault(e => (e.ProductId == productId && e.UserId == userId));
                if (cart == null)
                {
                    return false;
                }
                this._context.CartEntities.Remove(cart);
                await this._context.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ClearCart(string userId)
        {
            try
            {
                var cart = this._context.CartEntities.Where(e => e.UserId == userId);
                if(cart == null) return false;
                foreach(CartEntity item in cart)
                {
                    this._context.CartEntities.Remove(item);
                }
                await this._context.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public decimal CalculateTotal(string userId)
        {
            try
            {
                var cart = this._context.CartEntities
                     .Where(e => e.UserId == userId);
                decimal total = 0;
                foreach(CartEntity item in cart)
                {
                    var product = this._context.ProductEntities.FirstOrDefault(e => e.Id == item.ProductId);
                    total += (decimal)product!.Price * item.Qty;
                }
                return total;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
