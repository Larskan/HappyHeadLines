using Microsoft.EntityFrameworkCore;
using NewsletterService.Models;

namespace NewsletterService.Data;

public class NewsletterDbContext : DbContext
{
    public NewsletterDbContext(DbContextOptions<NewsletterDbContext> options) : base(options) { }

    public DbSet<NewsletterArticle> Articles => Set<NewsletterArticle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NewsletterArticle>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).IsRequired();
            entity.Property(a => a.Body).IsRequired();
            entity.Property(a => a.AuthorId).IsRequired();
            entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}