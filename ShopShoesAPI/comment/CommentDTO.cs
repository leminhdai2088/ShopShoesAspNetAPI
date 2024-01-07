namespace ShopShoesAPI.comment
{
    public class CommentDTO
    {
        public float Rating { get; set; }
        public string Content { get; set; } = string.Empty;
        public int ProductId { get; set; }
    }
}
