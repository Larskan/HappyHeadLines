using Microsoft.EntityFrameworkCore;
using PublisherService.Models;

namespace PublisherService.Data;

public class PublisherDbContext : DbContext
{
    public PublisherDbContext(DbContextOptions<PublisherDbContext> options) : base(options) { }
    public DbSet<PublishArticle> PublishArticles => Set<PublishArticle>();
}