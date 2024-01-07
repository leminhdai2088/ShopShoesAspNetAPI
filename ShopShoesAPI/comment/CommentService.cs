using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopShoesAPI.common;
using ShopShoesAPI.Data;
using ShopShoesAPI.product;

namespace ShopShoesAPI.comment
{
    public class CommentService: IComment
    {
        private readonly MyDbContext _context;
        private readonly AppSettings _appSettings;


        public CommentService(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            this._context = context;
            this._appSettings = optionsMonitor.CurrentValue;
        }
        public async Task<List<CommentDTO>> GetAllCommentsForProduct(int productId)
        {
            try
            {
                var comments = await this._context.CommentEntities
                    .Where(c => c.ProductId == productId)
                    .Select(c => new CommentDTO
                    {
                        Rating = c.Rating,
                        Content = c.Content,
                        ProductId = productId
                    }).ToListAsync();
                return comments; // Product updated successfully
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> CreateComment(CommentDTO comment)
        {
            try
            {
                var newComment = new CommentEntity
                {
                    Rating = comment.Rating,
                    Content = comment.Content,
                    ProductId = comment.ProductId,
                };

                this._context.Add(newComment);
                await this._context.SaveChangesAsync();

                return "Comment Successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteComment(int commentId)
        {
            try
            {
                var comment = await this._context.CommentEntities.FindAsync(commentId);
                if (comment == null)
                {
                    return false;
                }

                this._context.CommentEntities.Remove(comment);
                await this._context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> EditComment(int commentId, CommentDTO comment)
        {
            try
            {
                var existingComment = await this._context.CommentEntities.FindAsync(commentId);
                if (existingComment == null)
                {
                    return false;
                }

                existingComment.Rating = comment.Rating;
                existingComment.Content = comment.Content;

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
