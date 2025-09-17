using Microsoft.EntityFrameworkCore;

public class CommentDb : DbContext
{
    public CommentDb(DbContextOptions<CommentDb> options) : base(options) { }

    public DbSet<Comment> Comments { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}