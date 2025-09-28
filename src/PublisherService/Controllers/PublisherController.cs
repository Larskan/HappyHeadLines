using Microsoft.AspNetCore.Mvc;
using PublisherService.Models;
using PublisherService.Repositories;
using Shared;
using RabbitMQ.Client;
using PublisherService.Interfaces;
using System.Threading.Channels;
using System.Diagnostics;
using Serilog;

namespace PublisherService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublisherController : ControllerBase
{
    private readonly IPublishArticleRepository _repo;
    private readonly IConnection _rabbitConnection;
    private readonly ILogger<PublisherController> _logger;
    private static readonly ActivitySource activitySource = new("PublisherService");

    public PublisherController(IPublishArticleRepository repo, IConnection rabbitConnection, ILogger<PublisherController> logger)
    {
        _repo = repo;
        _rabbitConnection = rabbitConnection;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PublishArticle([FromBody] PublishArticle publishArticle)
    {
        // Distributed tracing, each request creates a span.
        using var activity = activitySource.StartActivity("PublishArticle");
        _logger.LogInformation("Publishing article {Title} by AuthorId {AuthorId}", publishArticle.Title, publishArticle.AuthorId);


        var savedArticle = await _repo.AddArticleAsync(publishArticle);

        using var channel = await _rabbitConnection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync("article_exchange", ExchangeType.Fanout, durable: true);

        // Trace Rabbit publish
        // Shows as a messaging event in Zipkin
        activity?.AddTag("messaging.system", "rabbitmq");
        activity?.AddTag("messaging.destination", "article_exchange");

        await RabbitHelper.PublishAsync(channel, "article_exchange", "", savedArticle);

        _logger.LogInformation("Published article {ArticleId} to RabbitMQ", savedArticle.PubArticleId);

        return Ok(savedArticle);
    }

}