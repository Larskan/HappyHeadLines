using ArticleService.Models;
using Microsoft.EntityFrameworkCore;
using ArticleService.Interfaces;

namespace ArticleService.Repositories;

public class ArticleRepository(IServiceProvider services) : IArticleRepository
{
    private readonly IServiceProvider _services = services;

    public async Task<Article?> GetByIdAsync(Guid id, string continent)
    {
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
        return await db.Articles.FindAsync(id);
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
        return article;
    }

    public async Task<bool> UpdateArticleAsync(Article article, string continent)
    {
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
        db.Articles.Update(article);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteArticleAsync(Guid id, string continent)
    {
        using var db = Helpers.DatabaseSelector.GetDbContext(_services, continent);
        var article = await db.Articles.FindAsync(id);
        if (article is null) return false;
        db.Articles.Remove(article);
        return await db.SaveChangesAsync() > 0;
    }
}