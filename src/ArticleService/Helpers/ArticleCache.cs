using Shared;
using ArticleService.Models;
using ArticleService.Interfaces;
using Prometheus;

namespace ArticleService.Helpers;

public class ArticleCache : BackgroundService
{
    private readonly IRedisHelper _redis;
    private readonly IArticleRepository _repo;
    private readonly ILogger<ArticleCache> _logger;
    private readonly TimeSpan _updateInternal = TimeSpan.FromMinutes(10); // Refresh every 10min
    private const string CacheKeyPrefix = "article:";
    private static readonly Counter CacheHits = Metrics.CreateCounter("article_cache_hits", "Number of cache hits for articles");
    private static readonly Counter CacheMisses = Metrics.CreateCounter("article_cache_misses", "Number of cache misses for articles");
    private static readonly Gauge CacheHitRatio = Metrics.CreateGauge("article_cache_hit_ratio", "Cache hit ratio for articles");
    private static readonly Gauge CacheMissRatio = Metrics.CreateGauge("article_cache_miss_ratio", "Cache miss ratio for articles");


    public ArticleCache(IRedisHelper redis, IArticleRepository repo, ILogger<ArticleCache> logger)
    {
        _redis = redis;
        _repo = repo;
        _logger = logger;
    }

    // Periodically refresh cache
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Keep looping as long as cancellation has not been requested
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await RefreshCacheAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ArticleCache");
            }
            await Task.Delay(_updateInternal, ct);
        }
    }

    private async Task RefreshCacheAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            // Fetch articles from the last 14 days
            var since = DateTime.UtcNow.AddDays(-14);
            var recentArticles = await _repo.GetArticlesSinceAsync(since);

            // Update cache
            foreach (var article in recentArticles)
            {
                string key = CacheKeyPrefix + article.Id;
                await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
            }
            _logger.LogInformation("ArticleCache updated with {Count} articles", recentArticles.Count);
        }   
    }

    // Try to get articles from cache first, otherwise from DB
    public async Task<Article?> GetArticlesFromCacheFirstAsync(int id, string continent)
    {
        string key = CacheKeyPrefix + id;
        var cached = await _redis.GetAsync<Article>(key);
        if (cached != null)
        {
            CacheHits.Inc();
            UpdateRatios();
            return cached;
        }
        CacheMisses.Inc();
        UpdateRatios();

        // On cache miss, fetch from DB
        var article = await _repo.GetByIdAsync(id, continent);
        if (article != null)
        {
            // Adds the db result to the cache for next time, with a 14-day expiration
            await _redis.SetAsync(key, article, TimeSpan.FromDays(14));
        }

        return article;
    }
    
    // Update cache hit/miss ratios for the metrics
    private void UpdateRatios()
    {
        var hits = CacheHits.Value;
        var misses = CacheMisses.Value;
        var total = hits + misses;

        if (total > 0)
        {
            double hitRatio = hits / total;
            double missRatio = misses / total;
            CacheHitRatio.Set(hitRatio);
            CacheMissRatio.Set(missRatio);
        }
    }
}