using ArticleService.Models;
using ArticleService.Repositories;
using Shared;

namespace ArticleService.Interfaces;

public interface IArticleService
{
    Task<ArticleDto?> GetByIdAsync(int id, string continent);
    Task<List<ArticleDto>> GetAllAsync(string continent);
    Task<ArticleDto> CreateArticleAsync(ArticleDto articleDto, string continent);
    Task<bool> UpdateArticleAsync(int id, ArticleDto articleDto, string continent);
    Task<bool> DeleteArticleAsync(int id, string continent);
}