
namespace CommentService.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string Author { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool IsFiltered { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}