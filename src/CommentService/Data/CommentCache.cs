using CommentService.Interfaces;
using Shared;
using Prometheus;

namespace CommentService.Data;

public class CommentCache
{
    private readonly IRedisHelper _redis;
    private readonly ICommentRepository _repo;
    private readonly string _recentArticlesKey = "recent_articles"; // Key to track recent articles with comments
    private readonly int _maxArticles = 30; //Only the 30 most recent articles with comments are cached
    private const string CacheKeyPrefix = "comments:";
    private static readonly Counter CacheHits = Metrics.CreateCounter("comment_cache_hits", "Number of cache hits for comments");
    private static readonly Counter CacheMisses = Metrics.CreateCounter("comment_cache_misses", "Number of cache misses for comments");
    private static readonly Gauge CacheHitRatio = Metrics.CreateGauge("comment_cache_hit_ratio", "Cache hit ratio for comments");
    private static readonly Gauge CacheMissRatio = Metrics.CreateGauge("comment_cache_miss_ratio", "Cache miss ratio for comments");


    public CommentCache(IRedisHelper redis, ICommentRepository repo)
    {
        _repo = repo;
        _redis = redis;
    }

    public async Task<List<CommentDto>> GetCommentsAsync(int articleId)
    {
        string key = CacheKeyPrefix + articleId;

        // Check redis/cache first
        var cached = await _redis.GetAsync<List<CommentDto>>(key);
        if (cached != null)
        {
            // If it finds stuff in the cache, add a cache hit to the metrics.
            CacheHits.Inc();
            UpdateRatios();
            await UpdateRecentArticlesAsync(articleId); // Marks this article as recently used
            return cached;
        }

        // Cache miss: Fetch from DB
        CacheMisses.Inc();
        UpdateRatios();
        var comments = await _repo.GetByArticleIdAsync(articleId);
        // Converts database stuff to DTOs
        var dtoList = comments.Select(c => new CommentDto(c.Id, c.ArticleId, c.Body, c.Author, c.CreatedAt)).ToList();

        // Save to redis cache
        await _redis.SetAsync(key, dtoList);
        await UpdateRecentArticlesAsync(articleId); // Marks this article as recently used

        // Trim LRU if needed, aka if we have more than 30 articles with comments in the cache, we trim the oldest ones away
        await TrimLRUAsync();

        return dtoList;
    }

    private async Task UpdateRecentArticlesAsync(int articleId)
    {
        // Fetch current recent list
        var recent = await _redis.GetAsync<List<string>>(_recentArticlesKey) ?? new List<string>();

        // Move or add articleId to the front
        recent.Remove(articleId.ToString()); // If this article is already in cache, but lower down, we remove it first
        recent.Insert(0, articleId.ToString()); // Then we add the article to the front, so it's the most recently used

        // Save back to Redis
        await _redis.SetAsync(_recentArticlesKey, recent); // No expiration, we want to keep track of recent articles comments indefinitely.. or until we reach max capacity.
    }

    private async Task TrimLRUAsync()
    {
        var recent = await _redis.GetAsync<List<string>>(_recentArticlesKey) ?? new List<string>();
        while (recent.Count > _maxArticles)
        {
            // Remove the least recently used articles
            var lru = recent.Last();
            recent.RemoveAt(recent.Count - 1);

            // Remove its comments from Redis
            await _redis.RemoveAsync($"comments:{lru}");
        }

        // Save updated list
        await _redis.SetAsync(_recentArticlesKey, recent);

    }

    // For prometheus and grafana metric stuff.
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