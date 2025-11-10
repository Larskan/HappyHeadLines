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
    private readonly IPublishArticleService _publishService;
    private readonly Serilog.ILogger _serilogger;
    private static readonly ActivitySource activitySource = new("PublisherService");

    public PublisherController(IPublishArticleService publishService, Serilog.ILogger serilogger)
    {
        _publishService = publishService;
        _serilogger = serilogger;
    }

    [HttpPost]
    public async Task<IActionResult> PublishArticle([FromBody] PublishArticle publishArticle)
    {
        using var activity = activitySource.StartActivity("PublishArticle", ActivityKind.Server);

        try
        {
            _serilogger.Information("Received request to publish article {@PublishArticle}", publishArticle);
            //Trace context
            activity?.SetTag("article.title", publishArticle.Title);
            activity?.SetTag("article.authorId", publishArticle.AuthorId);
            activity?.SetTag("article.publishedAt", publishArticle.PublishedAt);

            var savedArticle = await _publishService.PublishAsync(publishArticle);

            activity?.SetTag("article.id", savedArticle.PubArticleId);
            activity?.SetTag("article.status", "published");
            activity?.SetStatus(ActivityStatusCode.Ok);

            _serilogger.Information("Article published successfully with ID {ArticleId}", savedArticle.PubArticleId);

            return Ok(savedArticle);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _serilogger.Error(ex, "Error occurred while publishing article");
            return StatusCode(500, "Internal server error");
        }

    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllArticles()
    {
        using var activity = activitySource.StartActivity("GetAllArticles", ActivityKind.Server);

        try
        {
            var articles = await _publishService.GetAllAsync();
            _serilogger.Information("Fetched {Count} articles", articles.Count);
            return Ok(articles);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _serilogger.Error(ex, "Error while fetching all articles");
            return StatusCode(500, new { message = "Error while fetching articles." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetArticleById(int id)
    {
        using var activity = activitySource.StartActivity("GetArticleById", ActivityKind.Server);

        try
        {
            var article = await _publishService.GetByIdAsync(id);
            if (article == null)
            {
                _serilogger.Warning("Article with ID {ArticleId} not found", id);
                return NotFound(new { message = $"Article with ID {id} not found." });
            }

            _serilogger.Information("Fetched article with ID {ArticleId}", id);
            return Ok(article);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _serilogger.Error(ex, "Error while fetching article with ID {ArticleId}", id);
            return StatusCode(500, new { message = "Error while fetching article." });
        }
    }

}