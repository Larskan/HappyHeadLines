using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using Shared.Models;

namespace Shared;

public interface IArticleQueuePublisher
{
    Task PublishArticleAsync(PublishArticle article, CancellationToken ct = default);
}

public interface IArticleQueueSubscriber
{
    Task SubscribeAsync(Func<PublishArticle, CancellationToken, Task> handler, CancellationToken ct = default);
}

public class ArticleQueue : IArticleQueuePublisher, IArticleQueueSubscriber
{
    // private readonly IConnection _connection;
    private readonly IRabbitConnectionProvider _provider;
    private static readonly ActivitySource activitySource = new("Shared.ArticleQueue");
    public static RabbitHelper RabbitHelper { get; } = new();

    public ArticleQueue(IRabbitConnectionProvider provider)
    {
        _provider = provider;
    }

    private async Task<IConnection> GetConnectionAsync() => await _provider.GetConnectionAsync();

    public async Task PublishArticleAsync(PublishArticle article, CancellationToken ct = default)
    {
        using var channel = await (await GetConnectionAsync()).CreateChannelAsync(cancellationToken: ct);
        // Declare exchange
        await channel.ExchangeDeclareAsync("article_exchange", ExchangeType.Fanout, durable: true, cancellationToken: ct);

        // Publish message
        var helper = new RabbitHelper();

        // Start tracing activity
        using var activity = activitySource.StartActivity("PublishArticle");
        activity?.AddTag("messaging.system", "rabbitmq");
        activity?.AddTag("messaging.destination", "article_exchange");
        // Add article details as tags
        activity?.AddTag("article.id", article.PubArticleId.ToString());
        // Note: Avoid adding large content like article body to tracing tags
        activity?.AddTag("article.authorId", article.AuthorId);
        // Publish with tracing context
        await helper.PublishAsync(channel: channel, exchange: "article_exchange", routingKey: "", message: article, ct);
    }

    public async Task SubscribeAsync(Func<PublishArticle, CancellationToken, Task> handler, CancellationToken ct = default)
    {
        using var channel = await (await GetConnectionAsync()).CreateChannelAsync(cancellationToken: ct);
        // Declare exchange and queue
        await channel.ExchangeDeclareAsync("article_exchange", ExchangeType.Fanout, durable: true, cancellationToken: ct);

        var queue = await channel.QueueDeclareAsync(cancellationToken: ct);
        // Bind queue to exchange
        await channel.QueueBindAsync(queue.QueueName, "article_exchange", "", cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            // Start tracing activity
            using var activity = activitySource.StartActivity("ConsumeArticle");
            try
            {
                var body = args.Body.ToArray();
                var article = JsonSerializer.Deserialize<PublishArticle>(Encoding.UTF8.GetString(body)!);

                // Add tracing tags
                activity?.AddTag("messaging.system", "rabbitmq"); 
                activity?.AddTag("article.id", article?.PubArticleId.ToString()); 
                activity?.AddTag("article.authorId", article?.AuthorId);

                // Process the message
                if (article != null) await handler(article, ct);
                // Acknowledge message as processed
                await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firing the BasicNackAsync at {ex}");
                // Basic Negative Acknowledgement, requeue the message for another try
                await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true);
            }
        };
        // Start consuming messages
        await channel.BasicConsumeAsync(queue.QueueName, autoAck: false, consumer: consumer, cancellationToken: ct);

        // Keep service running
        await Task.Delay(Timeout.Infinite, ct);
    }
}