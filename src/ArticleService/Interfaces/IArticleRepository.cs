using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Interfaces;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(int id, string continent);
    Task<List<Article>> GetAllAsync(string continent);
    Task<Article> CreateArticleAsync(Article article, string continent);
    Task<bool> UpdateArticleAsync(Article article, string continent);
    Task<bool> DeleteArticleAsync(int id, string continent);
    Task<List<Article>> GetArticlesSinceAsync(DateTime since);
}