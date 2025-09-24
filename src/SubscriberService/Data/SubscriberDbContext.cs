using Microsoft.EntityFrameworkCore;
using SubscriberService.Models;

namespace SubscriberService.Data;

public class SubscriberDbContext : DbContext
{
    public SubscriberDbContext(DbContextOptions<SubscriberDbContext> options) : base(options) { }

    public DbSet<Subscriber> Subscribers => Set<Subscriber>();

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);

    //     modelBuilder.Entity<NewsletterArticle>(entity =>
    //     {
    //         entity.HasKey(a => a.Id);
    //         entity.Property(a => a.Title).IsRequired();
    //         entity.Property(a => a.Body).IsRequired();
    //         entity.Property(a => a.AuthorId).IsRequired();
    //         entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
    //     });
    // }
}