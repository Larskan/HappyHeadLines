namespace Shared;

public interface IRedisHelper
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<bool> ExistsAsync(string key);
    Task RemoveAsync(string key);
}