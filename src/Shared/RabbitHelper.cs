using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Shared;

// Message broker that helps systems communicate async via messages. 
// Uses: Load Balancing, Retry and durability(persistent queues), event driven systems(publish domain events in microservices)
public class RabbitHelper
{
    public async Task<IConnection> CreateConnectionAsync(string host = "localhost")
    {
        var factory = new ConnectionFactory() { HostName = host };
        return await factory.CreateConnectionAsync();
    }

    public async Task PublishAsync<T>(IChannel channel, string exchange, string routingKey, T message, CancellationToken ct = default)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        // Mark message as persistent, so it is not lost if RabbitMQ crashes during queueing
        var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };
        await channel.BasicPublishAsync(exchange, routingKey, mandatory: false, props, body, cancellationToken: ct);
    }
}