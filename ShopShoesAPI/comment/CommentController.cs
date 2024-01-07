using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.auth;
using ShopShoesAPI.Enums;
using ShopShoesAPI.product;
using ShopShoesAPI.user;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace ShopShoesAPI.comment
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IComment iComment;
        private readonly PayloadTokenDTO payloadTokenDTO;
        private readonly string userId;
        private readonly IAuth _iAuth;
        public CommentController(IComment iComment, IHttpContextAccessor httpContextAccessor, IAuth _iAuth)
        {
            this.iComment = iComment;
            this._iAuth = _iAuth;
            string authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
            if (authorizationHeader != null)
            {
                payloadTokenDTO = _iAuth.VerifyAccessToken(authorizationHeader!);
                userId = payloadTokenDTO?.Id;
            }

        }
        [HttpGet("Product/{productId}")]
        public async Task<ActionResult> GetAllCommentsForProduct(int productId)
        {
            try
            {
                var comments = await this.iComment.GetAllCommentsForProduct(productId);

                return Ok(comments);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult> CreateComment([FromBody] CommentDTO comment)
        {
            try
            {
                var result = await this.iComment.CreateComment(comment, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("/delete/{commentId}")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult> DeleteComment([FromRoute] int commentId)
        {
            try
            {
                var result = await this.iComment.DeleteComment(commentId, userId);
                if (result)
                {
                    return Ok("Comment deleted successfully");
                }
                return NotFound("Comment not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UpdateComment/{commentId}")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult> EditComment([FromRoute] int commentId, [FromBody] CommentDTO comment)
        {
            try
            {
                var result = await this.iComment.EditComment(commentId, comment, userId);
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
