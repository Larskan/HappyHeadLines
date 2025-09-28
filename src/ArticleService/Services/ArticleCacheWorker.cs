using Microsoft.EntityFrameworkCore;
using Shared;
using ArticleService.Helpers;
using ArticleService.Models;
using Serilog;

namespace ArticleService.Services;

public class ArticleCacheWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly RedisHelper _redis;
    private readonly ILogger<ArticleCacheWorker> _logger;

    public ArticleCacheWorker(IServiceProvider services, RedisHelper redis, ILogger<ArticleCacheWorker> logger)
    {
        _services = services;
        _redis = redis;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
            var since = DateTime.UtcNow.AddDays(-14);
            var recentArticles = await db.Articles.Where(a => a.CreatedAt >= since).ToListAsync(ct);

            foreach (var article in recentArticles)
            {
                var key = $"article:{article.Id}";
                await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
            }
            _logger.LogInformation("Cached {Count} articles from last 14 days", recentArticles.Count);

            await Task.Delay(TimeSpan.FromMinutes(30), ct);

        }
    }
}