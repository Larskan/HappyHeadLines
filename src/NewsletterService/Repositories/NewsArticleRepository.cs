using Microsoft.EntityFrameworkCore;
using NewsletterService.Data;
using NewsletterService.Models;
using NewsletterService.Interfaces;

namespace NewsletterService.Repositories;

public class NewsArticleRepository : INewsArticleRepository
{
    private readonly NewsletterDbContext _context;
    public NewsArticleRepository(NewsletterDbContext context) => _context = context;

    public async Task<NewsletterArticle> AddArticleAsync(NewsletterArticle article)
    {
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return article;
    }

    public async Task<List<NewsletterArticle>> GetAllArticlesAsync() => await _context.Articles.ToListAsync();
}