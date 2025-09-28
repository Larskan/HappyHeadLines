using Shared;
using ArticleService.Models;
using ArticleService.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using Serilog;

namespace ArticleService.Helpers;

public class ArticleCache : BackgroundService
{
    private readonly RedisHelper _redis;
    private readonly IArticleRepository _repo;
    private readonly ILogger<ArticleCache> _logger;
    private readonly TimeSpan _updateInternal = TimeSpan.FromMinutes(10); // Refresh every 10min
    private const string CacheKeyPrefix = "article:";

    public ArticleCache(RedisHelper redis, IArticleRepository repo, ILogger<ArticleCache> logger)
    {
        _redis = redis;
        _repo = repo;
        _logger = logger;
    }

    // Periodically refresh cache
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await RefreshCacheAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ArticleCache");
            }
            await Task.Delay(_updateInternal, ct);
        }
    }

    private async Task RefreshCacheAsync()
    {
        // Fetch articles from the last 14 days
        var cutoff = DateTime.UtcNow.AddDays(-14);
        var recentArticles = await _repo.GetArticlesSinceAsync(cutoff);

        foreach (var article in recentArticles)
        {
            string key = CacheKeyPrefix + article.Id;
            await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        }
        _logger.LogInformation("ArticleCache updated with {Count} articles", recentArticles.Count);

    }

    // Try to get articles from cache first
    public async Task<Article?> GetAsync(Guid id)
    {
        string key = CacheKeyPrefix + id;
        var article = await _redis.GetAsync<Article>(key);
        return article;
    }
}