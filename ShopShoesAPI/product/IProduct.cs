using System.Collections.Generic;
using System.Threading.Tasks;
using ShopShoesAPI.common;
namespace ShopShoesAPI.product
{
    public interface IProduct
    {
        
        Task<List<ProductDTO>> GetAllProducts(QueryAndPaginateDTO queryAndPaginate);
        Task<ProductDTO> GetProductById(int productId);
        Task<List<ProductDTO>> SearchProducts(string searchString);
        Task<string> CreateProduct(ProductDTO product);
        Task<bool> DeleteProduct(int productId);
        Task<bool> UpdateProduct(int productId, ProductDTO product);
    }
}

