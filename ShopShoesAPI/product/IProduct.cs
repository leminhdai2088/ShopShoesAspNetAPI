using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopShoesAPI.product
{
    public interface IProduct
    {
        {
        Task<List<ProductDTO>> GetAllProducts();
        Task<List<ProductDTO>> SearchProducts(string searchString);
        Task<string> CreateProduct(ProductDTO product);
        Task<bool> DeleteProduct(int productId);
    }
}
}
