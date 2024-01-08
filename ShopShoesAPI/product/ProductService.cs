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

        public ProductService(MyDbContext context)
        {
            this._context = context;
        }

        public ProductService()
        {
        }

        public async Task<List<ProductDTO>> GetAllProducts(QueryAndPaginateDTO queryAndPaginate)
        {
            try
            {
                var query = _context.ProductEntities
                    .Select(p => new ProductDTO
                    {
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Discount = p.Discount,
                        Image = p.Image,
                        Rating = p.Rating,
                        CategoryId = p.CategoryId
                    });

                if (queryAndPaginate != null)
                {
                    if (queryAndPaginate.pageIndex > 0 && queryAndPaginate.pageSize > 0)
                    {
                        var skipAmount = (queryAndPaginate.pageIndex - 1) * queryAndPaginate.pageSize;
                        query = query.Skip(skipAmount).Take(queryAndPaginate.pageSize);
                    }

                    if (!string.IsNullOrEmpty(queryAndPaginate.sortBy))
                    {
                        switch (queryAndPaginate.sortBy.ToLower())
                        {
                            case "price":
                                query = query.OrderBy(p => p.Price);
                                break;
                            default:
                                query = query.OrderBy(p => p.Id);
                                break;
                        }
                    }
                }

                var products = await query.ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductDTO> GetProductById(int productId)
        {
            try
            {
                var product = await _context.ProductEntities.FindAsync(productId);
                if (product == null)
                {
                    return null; // Product not found
                }

                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Discount = product.Discount,
                    Image = product.Image,
                    Rating = product.Rating,
                    CategoryId = product.CategoryId
                };

                return productDTO;
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
                };

                _context.Add(newProduct);
                await _context.SaveChangesAsync();

                return "Create product Successfully";
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
        public async Task<bool> UpdateProduct(int productId, ProductDTO product)
        {
            try
            {
                var existingProduct = await _context.ProductEntities.FindAsync(productId);
                if (existingProduct == null)
                {
                    return false;
                }

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Discount = product.Discount;
                existingProduct.Image = product.Image;
                existingProduct.Rating = product.Rating;
                existingProduct.CategoryId = product.CategoryId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<bool> UpdateProductQty(int productId, int qty)
        {
            try
            {
                var product = await this._context.ProductEntities.FirstOrDefaultAsync(e => e.Id == productId);
                if (product == null)
                {
                    throw new Exception("Product is not found");
                }
                product.Quantity -= qty;
                this._context.ProductEntities.Update(product);
                await this._context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsValidBuyQty(int productId, int qty)
        {
            try
            {
                var product = await this._context.ProductEntities.FirstOrDefaultAsync(e => e.Id == productId);
                if(product == null)
                {
                    throw new Exception("Product is not found");
                }
                if (qty > product.Quantity || qty < 0)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

