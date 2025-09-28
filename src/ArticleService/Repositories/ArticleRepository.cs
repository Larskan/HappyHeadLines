using ArticleService.Models;
using Microsoft.EntityFrameworkCore;
using ArticleService.Interfaces;
using Shared;

namespace ArticleService.Repositories;

public class ArticleRepository(IServiceProvider services, RedisHelper redis) : IArticleRepository
{
    private readonly IServiceProvider _services = services;
    private readonly RedisHelper _redis = redis;

    public async Task<Article?> GetByIdAsync(Guid id, string continent)
    {
        var key = $"article:{continent}:{id}";
        var cached = await _redis.GetAsync<Article>(key);
        if (cached != null) return cached;

        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
        var article = await db.Articles.FindAsync(id);

        if (article != null) await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        return article;
    }

    public async Task<List<Article>> GetAllAsync(string continent)
    {
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
        return await db.Articles.ToListAsync();
    }

    public async Task<Article> CreateArticleAsync(Article article, string continent)
    {
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
        db.Articles.Add(article);
        await db.SaveChangesAsync();

        var key = $"article:{continent}:{article.Id}";
        await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        return article;
    }

    public async Task<bool> UpdateArticleAsync(Article article, string continent)
    {
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
        db.Articles.Update(article);
        await db.SaveChangesAsync();

        var key = $"article:{continent}:{article.Id}";
        await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        return true;
    }

    public async Task<bool> DeleteArticleAsync(Guid id, string continent)
    {
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
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
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, "global");
        return await db.Articles.Where(a => a.PublishedAt >= since).ToListAsync();
    }
}