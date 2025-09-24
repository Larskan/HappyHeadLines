namespace DraftService.Models;

public class Draft
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}