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
    private readonly IConnection _connection;
    private static readonly ActivitySource activitySource = new("Shared.ArticleQueue");
    public static RabbitHelper RabbitHelper { get; } = new();

    public ArticleQueue(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishArticleAsync(PublishArticle article, CancellationToken ct = default)
    {
        using var channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync("article_exchange", ExchangeType.Fanout, durable: true, cancellationToken: ct);

        var helper = new RabbitHelper();

        using var activity = activitySource.StartActivity("PublishArticle");
        activity?.AddTag("messaging.system", "rabbitmq");
        activity?.AddTag("messaging.destination", "article_exchange");

        await helper.PublishAsync(channel: channel, exchange: "article_exchange", routingKey: "", message: article, ct);
    }

    public async Task SubscribeAsync(Func<PublishArticle, CancellationToken, Task> handler, CancellationToken ct = default)
    {
        using var channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync("article_exchange", ExchangeType.Fanout, durable: true, cancellationToken: ct);

        var queue = await channel.QueueDeclareAsync(cancellationToken: ct);
        await channel.QueueBindAsync(queue.QueueName, "article_exchange", "", cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            using var activity = activitySource.StartActivity("ConsumeArticle");
            try
            {
                var body = args.Body.ToArray();
                var article = JsonSerializer.Deserialize<PublishArticle>(Encoding.UTF8.GetString(body)!);

                activity?.AddTag("messaging.system", "rabbitmq");
                activity?.AddTag("article.id", article?.PubArticleId.ToString());
                activity?.AddTag("article.authorId", article?.AuthorId);

                if (article != null) await handler(article, ct);
                await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firing the BasicNackAsync at {ex}");
                await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true);
            }
        };
        await channel.BasicConsumeAsync(queue.QueueName, autoAck: false, consumer: consumer, cancellationToken: ct);

        // Keep service running
        await Task.Delay(Timeout.Infinite, ct);
    }
}