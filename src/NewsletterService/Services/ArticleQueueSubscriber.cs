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

public class ArticleQueueSubscriber : BackgroundService
{
    private readonly IConnection _rabbitConnection;
    private readonly INewsArticleRepository _newsArticleRepo;
    private readonly ILogger<ArticleQueueSubscriber> _logger;
    private static readonly ActivitySource activitySource = new("NewsletterService");

    public ArticleQueueSubscriber(IConnection rabbitConnection, INewsArticleRepository newsArticleRepository, ILogger<ArticleQueueSubscriber> logger)
    {
        _rabbitConnection = rabbitConnection;
        _newsArticleRepo = newsArticleRepository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var channel = await _rabbitConnection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(exchange: "article_exchange", type: ExchangeType.Fanout, durable: true, cancellationToken: ct);
        var queue = await channel.QueueDeclareAsync(cancellationToken: ct);
        await channel.QueueBindAsync(queue.QueueName, "article_exchange", "", cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            // Span is created for each consumed message.
            // Each is tagged with article.id, article.authorId and messaging.system=rabbitmq
            using var activity = activitySource.StartActivity("ConsumeArticle");
            try
            {
                var body = args.Body.ToArray();
                var article = JsonSerializer.Deserialize<NewsletterArticle>(Encoding.UTF8.GetString(body)!);
                _logger.LogInformation("Received article {Title} from Author {AuthorId}", article?.Title, article?.AuthorId);

                activity?.AddTag("messaging.system", "rabbitmq");
                activity?.AddTag("article.id", article?.Id.ToString());
                activity?.AddTag("article.authorId", article?.AuthorId);

                if (article != null) await _newsArticleRepo.AddArticleAsync(article);
                _logger.LogInformation("Saved article {ArticleId} to NewsletterDb", article?.Id);

                await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                // If processing fails, error log and requeue.
                _logger.LogError(ex, "Failed to process article message");
                await channel.BasicNackAsync(args.DeliveryTag, false, requeue: true);
            }
        };
        await channel.BasicConsumeAsync(
            queue: queue.QueueName,
            autoAck: false,
            consumer: consumer
        );
        await Task.Delay(-1, ct);
    }
}