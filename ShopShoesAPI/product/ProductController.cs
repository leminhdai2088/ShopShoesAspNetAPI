using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.user;
using ShopShoesAPI.common;
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
        public async Task<IActionResult> GetAllProducts([FromQuery] QueryAndPaginateDTO queryAndPaginate)
        {
            var products = await iProduct.GetAllProducts(queryAndPaginate);
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

        [HttpPost("/delete/{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            try
            {
                var result = await iProduct.DeleteProduct(productId);
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

        [HttpPut]
        [Route("UpdateProduct/{productId}")]
        public async Task<ActionResult> UpdateProduct(int productId, [FromBody] ProductDTO product)
        {
            try
            {
                var result = await iProduct.UpdateProduct(productId, product);
                if (result)
                {
                    return Ok("Product updated successfully");
                }
                return NotFound("Product not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("/{productId}")]
        public async Task<ActionResult> GetProductById(int productId)
        {
            var product = await iProduct.GetProductById(productId);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound("Product not found");
        }
    }
}
