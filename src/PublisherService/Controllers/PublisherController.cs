using Microsoft.AspNetCore.Mvc;
using PublisherService.Models;
using PublisherService.Repositories;
using Shared;
using RabbitMQ.Client;
using PublisherService.Interfaces;
using System.Threading.Channels;

namespace PublisherService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublisherController : ControllerBase
{
    private readonly IPublishArticleRepository _repo;
    private readonly IConnection _rabbitConnection;

    public PublisherController(IPublishArticleRepository repo, IConnection rabbitConnection)
    {
        _repo = repo;
        _rabbitConnection = rabbitConnection;
    }

    [HttpPost]
    public async Task<IActionResult> PublishArticle([FromBody] PublishArticle publishArticle)
    {
        var savedArticle = await _repo.AddArticleAsync(publishArticle);
        using var channel = await _rabbitConnection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync("article_exchange", ExchangeType.Fanout, durable: true);
        await RabbitHelper.PublishAsync(channel, "article_exchange", "", savedArticle);

        return Ok(savedArticle);
    }

}