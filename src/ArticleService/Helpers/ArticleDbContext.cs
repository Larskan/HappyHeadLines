using Microsoft.EntityFrameworkCore;
using ArticleService.Models;

namespace ArticleService.Models;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }
    public DbSet<Article> Articles => Set<Article>();
}