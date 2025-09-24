using System.Data.Common;

namespace NewsletterService.Models;

public class NewsletterArticle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}