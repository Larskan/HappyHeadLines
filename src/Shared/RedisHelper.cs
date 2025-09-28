using StackExchange.Redis;
using System.Text.Json;

namespace Shared;

public class RedisHelper
{
    private readonly IDatabase _db;

    public RedisHelper(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task<bool> ExistsAsync(string key) => await _db.KeyExistsAsync(key);
    public async Task RemoveAsync(string key) => await _db.KeyDeleteAsync(key);
}