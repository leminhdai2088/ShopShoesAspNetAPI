using ShopShoesAPI.comment;

namespace ShopShoesAPI.comment
{
    public interface IComment
    {
        Task<IEnumerable<object>> GetAllCommentsForProduct(int productId);
        Task<string> CreateComment(CommentDTO comment, string userId);
        Task<bool> DeleteComment(int commentId, string userId);
        Task<bool> EditComment(int commentId, CommentDTO comment, string userId);
    }
}
