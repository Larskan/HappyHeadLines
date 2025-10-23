using ArticleService.Models;
using Microsoft.EntityFrameworkCore;
using ArticleService.Interfaces;
using Shared;
using ArticleService.Helpers;

namespace ArticleService.Repositories;

public class ArticleRepository: IArticleRepository
{
    private readonly IServiceProvider _services ;
    private readonly IRedisHelper _redis;

    public ArticleRepository(IServiceProvider services, IRedisHelper redis)
    {
        _services = services;
        _redis = redis;
    }
    
    private ArticleDbContext GetContext(string continent) => DatabaseSelector.GetDbContext(_services, continent);
    

    public async Task<Article?> GetByIdAsync(int id, string continent)
    {
        var key = $"article:{continent}:{id}";
        var cached = await _redis.GetAsync<Article>(key);
        if (cached != null) return cached;

        using var db = GetContext(continent);
        var article = await db.Articles.FindAsync(id);

        if (article != null) await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        return article;
    }

    public async Task<List<Article>> GetAllAsync(string continent)
    {
        using var db = GetContext(continent);
        return await db.Articles.ToListAsync();
    }

    public async Task<Article> CreateArticleAsync(Article article, string continent)
    {
        using var db = GetContext(continent);
        db.Articles.Add(article);
        await db.SaveChangesAsync();

        var key = $"article:{continent}:{article.Id}";
        await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        return article;
    }

    public async Task<bool> UpdateArticleAsync(Article article, string continent)
    {
        using var db = GetContext(continent);
        db.Articles.Update(article);
        await db.SaveChangesAsync();

        var key = $"article:{continent}:{article.Id}";
        await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        return true;
    }

    public async Task<bool> DeleteArticleAsync(int id, string continent)
    {
        using var db = GetContext(continent);
        var article = await db.Articles.FindAsync(id);
        if (article is null) return false;
        db.Articles.Remove(article);
        var deleted = await db.SaveChangesAsync() > 0;

        if (deleted)
        {
            var key = $"article:{continent}:{id}";
            await _redis.RemoveAsync(key);
        }
        return deleted;
    }

    public async Task<List<Article>> GetArticlesSinceAsync(DateTime since)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
        return await db.Articles.Where(a => a.PublishedAt >= since).ToListAsync();
    }
}