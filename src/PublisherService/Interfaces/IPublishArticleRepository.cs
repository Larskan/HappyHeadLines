using PublisherService.Models;

namespace PublisherService.Interfaces;

public interface IPublishArticleRepository
{
    Task<PublishArticle> AddArticleAsync(PublishArticle publishArticle);
    Task<PublishArticle?> GetArticleByIdAsync(int id);
    Task<List<PublishArticle>> GetAllArticlesAsync();
}