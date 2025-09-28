using ArticleService.Models;
using ArticleService.Repositories;
using ArticleService.Helpers;
using ArticleService.Interfaces;
using Shared;

namespace ArticleService.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _repository;
    private readonly ArticleCache _cache;

    public ArticleService(IArticleRepository repository, ArticleCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    // Cache can store full article, but service still returns correct DTO
    public async Task<ArticleDto?> GetByIdAsync(Guid id, string continent)
    {
        // Try cache first
        var cached = await _cache.GetAsync(id);
        if (cached != null) return ToDto(cached);

        // fallback to DB
        var article = await _repository.GetByIdAsync(id, continent);
        if (article == null) return null;
        return ToDto(article);
    }
    public async Task<List<ArticleDto>> GetAllAsync(string continent)
    {
        var articles = await _repository.GetAllAsync(continent);
        return articles.Select(ToDto).ToList();
    }

    public async Task<ArticleDto> CreateArticleAsync(ArticleDto articleDto, string continent)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = articleDto.Title,
            Body = articleDto.Body,
            Author = articleDto.Author,
            PublishedAt = articleDto.PublishedAt
        };
        var created = await _repository.CreateArticleAsync(article, continent);
        return ToDto(created);
    }

    public async Task<bool> UpdateArticleAsync(Guid id, ArticleDto articleDto, string continent)
    {
        var existingArticle = await _repository.GetByIdAsync(id, continent);
        if (existingArticle is null) return false;

        existingArticle.Title = articleDto.Title;
        existingArticle.Body = articleDto.Body;
        existingArticle.Author = articleDto.Author;
        existingArticle.PublishedAt = articleDto.PublishedAt;
        existingArticle.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateArticleAsync(existingArticle, continent);
    }

    public async Task<bool> DeleteArticleAsync(Guid id, string continent) =>
        await _repository.DeleteArticleAsync(id, continent);
    
    // Mapping method from Article to ArticleDto
    private static ArticleDto ToDto(Article a) =>
        new ArticleDto(a.Id, a.Title, a.Body, a.PublishedAt, a.Author);
    
        
}