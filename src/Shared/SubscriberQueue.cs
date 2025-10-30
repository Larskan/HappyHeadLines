using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using Shared.Models;

namespace Shared;

// Use this if you want to publish a subscription message
public interface ISubscriberQueuePublisher
{
    Task PublishSubscriberAsync(Subscription subscription, CancellationToken ct = default);
}

// Use this to subscribe to subscription messages
public interface ISubscriberQueueSubscriber
{
    Task SubscribeSubscriberAsync(Func<Subscription, CancellationToken, Task> handler, CancellationToken ct = default);
}

public class SubscriberQueue : ISubscriberQueuePublisher, ISubscriberQueueSubscriber
{
    private readonly IRabbitConnectionProvider _provider;
    private static readonly ActivitySource activitySource = new("Shared.SubscriberQueue");
    public static RabbitHelper rabbitHelper { get; } = new();
    private const string ExchangeName = "subscription_exchange";

    public SubscriberQueue(IRabbitConnectionProvider provider)
    {
        _provider = provider;
    }

    private async Task<IConnection> GetConnectionAsync() => await _provider.GetConnectionAsync();

    // Called by SubscriberService when someone subscribes
    public async Task PublishSubscriberAsync(Subscription subscription, CancellationToken ct = default)
    {
        var connection = await GetConnectionAsync();
        using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        // Durable fanout so all interested parties get it
        await channel.ExchangeDeclareAsync("subscription_exchange", ExchangeType.Fanout, durable: true, cancellationToken: ct);

        // Start tracing activity
        using var activity = activitySource.StartActivity("PublishSubscription");
        activity?.AddTag("messaging.system", "rabbitmq");
        activity?.AddTag("messaging.destination", ExchangeName);
        activity?.AddTag("subscription.id", subscription.SubscriberId.ToString());
        activity?.AddTag("subscription.email", subscription.Email ?? string.Empty);
        // Helper to publish with tracing context and properties

        await rabbitHelper.PublishAsync(channel: channel, exchange: ExchangeName, routingKey: "", message: subscription, ct);
    }
    
    // Called by Newsletter to receive events
    public async Task SubscribeSubscriberAsync(Func<Subscription, CancellationToken, Task> handler, CancellationToken ct = default)
    {
        var connection = await GetConnectionAsync();
        var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, durable: true, cancellationToken: ct);

        var queue = await channel.QueueDeclareAsync(queue: "subscription_exchange", durable: true, cancellationToken: ct);
        // Bind queue to exchange
        await channel.QueueBindAsync(queue.QueueName, ExchangeName, "", cancellationToken: ct);

        // Avoiding overload
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 5, global: false, cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            // Start tracing activity
            using var activity = activitySource.StartActivity("ConsumeSubscription");
            try
            {
                var body = args.Body.ToArray();
                var subscription = JsonSerializer.Deserialize<Subscription>(Encoding.UTF8.GetString(body)!);

                // Add tracing tags
                activity?.AddTag("messaging.system", "rabbitmq");
                activity?.AddTag("subscription.id", subscription?.SubscriberId.ToString()); 
                activity?.AddTag("subscription.email", subscription?.Email ?? string.Empty);

                // Process the message
                if (subscription != null) await handler(subscription, ct);
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