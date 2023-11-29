using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShopShoesAPI.product
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct iProduct;
        public ProductController(IProduct iProduct)
        {
            this.iProduct = iProduct;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await iProduct.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("/search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string searchString)
        {
            var products = await iProduct.SearchProducts(searchString);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] ProductDTO product)
        {
            try
            {
                var result = await iProduct.CreateProduct(product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/delete")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            try
            {
                var result = await _productService.DeleteProduct(productId);
                if (result)
                {
                    return Ok("Product deleted successfully");
                }
                return NotFound("Product not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
