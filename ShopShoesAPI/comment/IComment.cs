using ShopShoesAPI.comment;

namespace ShopShoesAPI.comment
{
    public interface IComment
    {
        Task<List<CommentDTO>> GetAllCommentsForProduct(int productId);
        Task<string> CreateComment(CommentDTO comment);
        Task<bool> DeleteComment(int commentId);
        Task<bool> EditComment(int commentId, CommentDTO comment);
    }
}
