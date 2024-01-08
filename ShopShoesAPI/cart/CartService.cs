using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopShoesAPI.Data;
using ShopShoesAPI.order;
using ShopShoesAPI.user;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopShoesAPI.cart
{
    public class CartService : ICart
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<UserEnityIndetity> userManager;
        public CartService(MyDbContext context, UserManager<UserEnityIndetity> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }
        public List<CartDTO> GetCartItems()
        {
            try
            {
                var cart = this.httpContextAccessor?.HttpContext?.Session.GetString("Cart");
                var options = new JsonSerializerOptions
                {
                    // Các tùy chọn
                    ReferenceHandler = ReferenceHandler.Preserve,
                    
                };
                var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart, options) : null;
                return cartItems;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool AddOneItemToCart(int productId)
        {
            try
            {
                var cart = this.httpContextAccessor?.HttpContext?.Session.GetString("Cart");

                // Parse chuỗi json cart thành kiểu list cartDto
                var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

                var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                }
                else
                {
                    var newItem = new CartDTO { 
                        ProductId = productId,
                        Quantity = 1,
                    };
                    cartItems.Add(newItem);
                }

                this.httpContextAccessor?.HttpContext?.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
                return true;
            }catch(Exception ex) 
            {
                throw new Exception(ex.Message); 
            }
        }

        public bool MinusOneItemToCart(int productId)
        {
            try
            {
                var cart = this.httpContextAccessor?.HttpContext?.Session.GetString("Cart");

                // Parse chuỗi json cart thành kiểu list cartDto
                var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

                var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity -= 1;

                    if(existingItem.Quantity <= 0) RemoveFromCart(productId);
                    return true;
                }

                this.httpContextAccessor?.HttpContext?.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool AddToCart(CartDTO item)
        {
            try
            {
                var cart = this.httpContextAccessor?.HttpContext?.Session.GetString("Cart");

                // Parse chuỗi json cart thành kiểu list cartDto
                var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

                var existingItem = cartItems.FirstOrDefault(c => c.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = item.Quantity;
                }
                else
                {
                    var newItem = new CartDTO
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                    };
                    cartItems.Add(newItem);
                }

                this.httpContextAccessor?.HttpContext?.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool RemoveFromCart(int productId)
        {
            try
            {
                var cart = this.httpContextAccessor?.HttpContext?.Session.GetString("Cart");
                var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : null;
                if (cartItems == null)
                {
                    return false;
                }
                var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                    this.httpContextAccessor?.HttpContext?.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
                    return true;
                }
                return false;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool ClearCart()
        {
            try
            {
                this.httpContextAccessor?.HttpContext?.Session.Remove("Cart");
                return true;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal CalculateTotal()
        {
            try
            {
                var cart = this.httpContextAccessor?.HttpContext?.Session.GetString("Cart");
                var cartItems = cart != null ? JsonSerializer.Deserialize<List<CartDTO>>(cart) : new List<CartDTO>();

                // decimal total = (decimal)cartItems.Sum(item => item.Price * item.Quantity);
                var tasks = cartItems.Select(item =>
                {
                    var product = this._context.ProductEntities
                        .FirstOrDefault(e => e.Id == item.ProductId);
                    return product.Price * item.Quantity;
                });

                // Wait for all tasks to complete and then sum the results
                decimal total = (decimal)tasks.Sum();
                return total;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
