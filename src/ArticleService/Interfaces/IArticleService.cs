using ArticleService.Models;
using ArticleService.Repositories;
using Shared;

namespace ArticleService.Interfaces;

public interface IArticleService
{
    Task<ArticleDto?> GetByIdAsync(Guid id, string continent);
    Task<List<ArticleDto>> GetAllAsync(string continent);
    Task<ArticleDto> CreateArticleAsync(ArticleDto articleDto, string continent);
    Task<bool> UpdateArticleAsync(Guid id, ArticleDto articleDto, string continent);
    Task<bool> DeleteArticleAsync(Guid id, string continent);
}