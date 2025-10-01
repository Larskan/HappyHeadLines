namespace Shared.Models;


public class PublishArticle
{
    public int PubArticleId { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}

