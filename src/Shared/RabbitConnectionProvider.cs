using RabbitMQ.Client;

namespace Shared;

public interface IRabbitConnectionProvider
{
    Task<IConnection> GetConnectionAsync(CancellationToken ct = default);
}

public class RabbitConnectionProvider : IRabbitConnectionProvider, IAsyncDisposable
{
    private readonly string _host;
    private IConnection? _connection;

    public RabbitConnectionProvider(string host = "rabbitmq")
    {
        _host = host;
    }

    public async Task<IConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        if (_connection != null && _connection.IsOpen)
            return _connection;

        var factory = new ConnectionFactory() { HostName = _host };
        _connection = await factory.CreateConnectionAsync(cancellationToken: ct);
        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if(_connection != null && _connection.IsOpen)
        {
            await Task.Run(() => _connection.CloseAsync());
            _connection.Dispose();
        }
    }
}