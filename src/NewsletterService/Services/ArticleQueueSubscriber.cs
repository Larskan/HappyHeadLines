using NewsletterService.Interfaces;
using NewsletterService.Models;
using NewsletterService.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;
using System.Text.Json;

namespace NewsletterService.Services;

public class ArticleQueueSubscriber : BackgroundService
{
    private readonly IConnection _rabbitConnection;
    private readonly INewsArticleRepository _newsArticleRepo;

    public ArticleQueueSubscriber(IConnection rabbitConnection, INewsArticleRepository newsArticleRepository)
    {
        _rabbitConnection = rabbitConnection;
        _newsArticleRepo = newsArticleRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await using var channel = await _rabbitConnection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(exchange: "article_exchange", type: ExchangeType.Fanout, durable: true, cancellationToken: ct);

        // Declare a queue and get its name
        var queueDeclare = await channel.QueueDeclareAsync(
            queue: "", //Empty means RabbitMQ generates one.
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: ct
        );
        var queueName = queueDeclare.QueueName;

        await channel.QueueBindAsync(
            queue: queueName,
            exchange: "article_exchange",
            routingKey: "",
            cancellationToken: ct
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var article = JsonSerializer.Deserialize<NewsletterArticle>(Encoding.UTF8.GetString(body));
                if (article != null) await _newsArticleRepo.AddArticleAsync(article);

                await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        };
        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct
        );
        await Task.Delay(-1, ct);
    }
}