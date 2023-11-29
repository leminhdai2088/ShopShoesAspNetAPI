using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopShoesAPI.common;
using ShopShoesAPI.Data;
using ShopShoesAPI.email;
using ShopShoesAPI.product;

namespace ShopShoesAPI.user
{
    public class ProductService: IProduct
    {
        private readonly MyDbContext _context;
        private readonly AppSettings _appSettings;
        
        
        public ProductService(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            this._context = context;
            this._appSettings = optionsMonitor.CurrentValue;
        }

        public async Task<List<ProductDTO>> GetAllProducts()
        {
            try
            {
                var products = _context.ProductEntities
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Discount = p.Discount,
                    Image = p.Image,
                    Rating = p.Rating,
                    CategoryId = p.CategoryId
                    // Map other properties if necessary
                })
                .ToList();

                return products;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ProductDTO>> SearchProducts(string searchString)
        {
            try
            {
                var products = await _context.ProductEntities
                    .Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString))
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Discount = p.Discount,
                        Image = p.Image,
                        Rating = p.Rating,
                        CategoryId = p.CategoryId
                    })
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> CreateProduct(ProductDTO product)
        {
            try
            {
                var newProduct = new ProductEntity
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Discount = product.Discount,
                    Image = product.Image,
                    Rating = product.Rating,
                    CategoryId = product.CategoryId
                    // Map other properties if necessary
                };

                _context.Add(newProduct);
                await _context.SaveChangesAsync();

                return "Create product Successfully"; // You may return the created product data if needed
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                var product = await _context.ProductEntities.FindAsync(productId);
                if (product == null)
                {
                    return false;
                }

                _context.ProductEntities.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

