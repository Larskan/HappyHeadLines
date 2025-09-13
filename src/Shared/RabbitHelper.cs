using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Shared;

public static class RabbitHelper
{
    public static async Task<IConnection> CreateConnectionAsync(string host = "localhost")
    {
        var factory = new ConnectionFactory() { HostName = host };
        return await factory.CreateConnectionAsync();
    }

    public static async Task PublishAsync<T>(IChannel channel, string exchange, string routingKey, T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };
        await channel.BasicPublishAsync(exchange, routingKey, mandatory: false, props, body);
    }
}