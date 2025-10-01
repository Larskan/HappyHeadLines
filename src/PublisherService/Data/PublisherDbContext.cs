using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace PublisherService.Data;

public class PublisherDbContext : DbContext
{
    public PublisherDbContext(DbContextOptions<PublisherDbContext> options) : base(options) { }
    public DbSet<PublishArticle> PublishArticles => Set<PublishArticle>();
}