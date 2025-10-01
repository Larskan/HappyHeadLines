using Shared.Models;
using PublisherService.Interfaces;
using PublisherService.Data;
using Microsoft.EntityFrameworkCore;

namespace PublisherService.Repositories;


public class PublishArticleRepository : IPublishArticleRepository
{
    private readonly PublisherDbContext _context;
    public PublishArticleRepository(PublisherDbContext context) => _context = context;

    public async Task<PublishArticle> AddArticleAsync(PublishArticle publishArticle)
    {
        _context.PublishArticles.Add(publishArticle);
        await _context.SaveChangesAsync();
        return publishArticle;
    }

    public async Task<PublishArticle?> GetArticleByIdAsync(int id) =>
        await _context.PublishArticles.FirstOrDefaultAsync(a => a.PubArticleId == id);

    public async Task<List<PublishArticle>> GetAllArticlesAsync() => await _context.PublishArticles.ToListAsync();
}