using Microsoft.EntityFrameworkCore;
using ArticleService.Models;

namespace ArticleService.Models;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }
    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>().HasKey(a => a.Id);
        modelBuilder.Entity<Article>().Property(a => a.Title).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<Article>().Property(a => a.Body).IsRequired();
        modelBuilder.Entity<Article>().Property(a => a.Author).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Article>().Property(a => a.PublishedAt).IsRequired();
    }
}