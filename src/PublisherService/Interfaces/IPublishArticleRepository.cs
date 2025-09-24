using PublisherService.Models;

namespace PublisherService.Interfaces;

public interface IPublishArticleRepository
{
    Task<PublishArticle> AddArticleAsync(PublishArticle publishArticle);
    Task<PublishArticle?> GetArticleByIdAsync(Guid id);
    Task<List<PublishArticle>> GetAllArticlesAsync();
}