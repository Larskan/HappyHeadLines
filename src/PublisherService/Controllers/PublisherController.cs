using Microsoft.AspNetCore.Mvc;
using PublisherService.Repositories;
using Shared;
using Shared.Models;
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
    public async Task<IActionResult> PublishArticle([FromBody] PublishArticle publishArticle, [FromServices] IArticleQueuePublisher queue)
    {
        var savedArticle = await _repo.AddArticleAsync(publishArticle);

        // Publish via shared queue
        await queue.PublishArticleAsync(savedArticle);
        _logger.LogInformation("Publishing article {ArticleId} to ArticleQueue", savedArticle.PubArticleId);

        return Ok(savedArticle);
    }

}