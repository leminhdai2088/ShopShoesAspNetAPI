using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.product;
using System.Threading.Tasks;

namespace ShopShoesAPI.comment
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IComment iComment;
        public CommentController(IComment iComment)
        {
            this.iComment = iComment;
        }
        [HttpGet]
        [Route("Product/{productId}")]
        public async Task<ActionResult> GetAllCommentsForProduct(int productId)
        {
            try
            {
                var comments = await iComment.GetAllCommentsForProduct(productId);

                return Ok(comments);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult> CreateComment([FromBody] CommentDTO comment)
        {
            try
            {
                var result = await iComment.CreateComment(comment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/delete")]
        public async Task<ActionResult> DeleComment(int productId)
        {
            try
            {
                var result = await iComment.DeleteComment(productId);
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
        [Route("UpdateComment/{commentId}")]
        public async Task<ActionResult> EditComment(int commentId, [FromBody] CommentDTO comment)
        {
            try
            {
                var result = await iComment.EditComment(commentId, comment);
                if (result)
                {
                    return Ok("Comment has edited successfully");
                }
                return NotFound("Comment not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
