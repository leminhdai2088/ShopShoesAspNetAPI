using System.Collections.Generic;
using System.Threading.Tasks;
using ShopShoesAPI.common;
namespace ShopShoesAPI.product
{
    public interface IProduct
    {
        
        public Task<List<ProductDTO>> GetAllProducts(QueryAndPaginateDTO queryAndPaginate);
        public Task<ProductDTO> GetProductById(int productId);
        public Task<List<ProductDTO>> SearchProducts(string searchString);
        public Task<string> CreateProduct(ProductDTO product);
        public Task<bool> DeleteProduct(int productId);
        public Task<bool> UpdateProduct(int productId, ProductDTO product);
        public Task<bool> UpdateProductQty(int productId, int qty);
        public Task<bool> IsValidBuyQty(int productId, int qty);
    }
}

