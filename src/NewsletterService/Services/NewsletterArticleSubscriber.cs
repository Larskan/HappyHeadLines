using NewsletterService.Interfaces;
using NewsletterService.Models;
using NewsletterService.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace NewsletterService.Services;

public class NewsletterArticleSubscriber : BackgroundService
{
    private readonly IArticleQueueSubscriber _queue;
    private readonly INewsArticleRepository _repo;

    public NewsletterArticleSubscriber(IArticleQueueSubscriber queue, INewsArticleRepository repo)
    {
        _queue = queue;
        _repo = repo;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _queue.SubscribeAsync(async (publishArticle, ct) =>
        {
            // Map PublishArticle -> NewsletterArticle
            var newsletterArticle = new NewsletterArticle
            {
                Id = publishArticle.PubArticleId,
                AuthorId = publishArticle.AuthorId,
                Title = publishArticle.Title,
                Body = publishArticle.Body,
                CreatedAt = publishArticle.PublishedAt
            };
            await _repo.AddArticleAsync(newsletterArticle);

        }, ct);
    }
}
