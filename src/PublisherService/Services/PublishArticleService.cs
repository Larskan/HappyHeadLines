using Shared.Models;
using PublisherService.Interfaces;
using PublisherService.Data;
using Microsoft.EntityFrameworkCore;
using Shared;


namespace PublisherService.Services;
public class PublishArticleService : IPublishArticleService
{
    private readonly IPublishArticleRepository _repository;
    private readonly IArticleQueuePublisher _queuePublisher;
    private readonly Serilog.ILogger _serilogger;
    public PublishArticleService(IPublishArticleRepository repository, IArticleQueuePublisher queuePublisher, Serilog.ILogger serilogger)
    {
        _repository = repository;
        _queuePublisher =  queuePublisher;
        _serilogger = serilogger;
    }

    public async Task<PublishArticle> PublishAsync(PublishArticle article)
    {
        _serilogger.Information("Saving article: {Title}", article.Title);
        var savedArticle = await _repository.AddArticleAsync(article);
        _serilogger.Information("Publishing article {ArticleId} to ArticleQueue", savedArticle.PubArticleId);
        await _queuePublisher.PublishArticleAsync(savedArticle);
        return savedArticle;
    }

    public async Task<PublishArticle?> GetByIdAsync(int id) => await _repository.GetArticleByIdAsync(id);

    public async Task<List<PublishArticle>> GetAllAsync() => await _repository.GetAllArticlesAsync();
}