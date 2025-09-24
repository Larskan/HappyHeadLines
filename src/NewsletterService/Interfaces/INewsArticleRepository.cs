using NewsletterService.Models;

namespace NewsletterService.Interfaces;

public interface INewsArticleRepository
{
    Task<NewsletterArticle> AddArticleAsync(NewsletterArticle article);
    Task<List<NewsletterArticle>> GetAllArticlesAsync();
}