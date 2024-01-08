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
        public async Task<IEnumerable<object>> GetAllCommentsForProduct(int productId)
        {
            try
            {
                var comments = await this._context.CommentEntities
                    .Where(c => c.ProductId == productId)
                    .Include(c => c.User)
                    .Select(c => new
                    {
                         c.Id,
                         c.Rating,
                         c.Content,
                         c.ProductId,
                         c.UserId,
                         c.User.FullName,
                         c.User.Email
                    }).ToListAsync();
                return comments; // Product updated successfully
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> CreateComment(CommentDTO comment, string userId)
        {
            try
            {
                var newComment = new CommentEntity
                {
                    Rating = comment.Rating,
                    Content = comment.Content,
                    ProductId = comment.ProductId,
                };
                newComment.UserId = userId;

                this._context.Add(newComment);
                await this._context.SaveChangesAsync();

                return "Comment Successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteComment(int commentId, string userId)
        {
            try
            {
                var comment = await this._context.CommentEntities.FindAsync(commentId);
                if (comment == null || comment.UserId != userId)
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

        public async Task<bool> EditComment(int commentId, CommentDTO comment, string userId)
        {
            try
            {
                var existingComment = await this._context.CommentEntities.FindAsync(commentId);
                if (existingComment == null || existingComment.UserId != userId)
                {
                    return false;
                }

                existingComment.Rating = comment.Rating;
                existingComment.Content = comment.Content;
                this._context.CommentEntities.Update(existingComment);
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
