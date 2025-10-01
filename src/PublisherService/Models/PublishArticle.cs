namespace PublisherService.Models;


public class PublishArticle
{
    public int PubArticleId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public int AuthorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

