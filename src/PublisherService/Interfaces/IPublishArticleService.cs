using Shared.Models;

namespace PublisherService.Interfaces;

public interface IPublishArticleService
{
    Task<PublishArticle> PublishAsync(PublishArticle publishArticle);
    Task<List<PublishArticle>> GetAllAsync();
    Task<PublishArticle?> GetByIdAsync(int id);
}