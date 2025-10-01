using CommentService.Interfaces;
using Shared;

namespace CommentService.Data;

public class CommentCache
{
    private readonly IRedisHelper _redis;
    private readonly ICommentRepository _repo;
    private readonly string _recentArticlesKey = "recent_articles";
    private readonly int _maxArticles = 30;

    public CommentCache(IRedisHelper redis, ICommentRepository repo)
    {
        _repo = repo;
        _redis = redis;
    }

    public async Task<List<CommentDto>> GetCommentsAsync(int articleId)
    {
        string key = $"comments:{articleId}";

        // Check redis/cache first
        var cached = await _redis.GetAsync<List<CommentDto>>(key);
        if (cached != null)
        {
            await UpdateRecentArticlesAsync(articleId);
            return cached;
        }

        // Cache miss: Fetch from DB
        var comments = await _repo.GetByArticleIdAsync(articleId);
        var dtoList = comments.Select(c => new CommentDto(c.Id, c.ArticleId, c.Body, c.Author, c.CreatedAt)).ToList();

        // Save to redis
        await _redis.SetAsync(key, dtoList);
        await UpdateRecentArticlesAsync(articleId);

        // Trim LRU if needed
        await TrimLRUAsync();

        return dtoList;
    }

    private async Task UpdateRecentArticlesAsync(int articleId)
    {
        // Fetch current recent list
        var recent = await _redis.GetAsync<List<string>>(_recentArticlesKey) ?? new List<string>();

        // Move or add articleId to the front
        recent.Remove(articleId.ToString());
        recent.Insert(0, articleId.ToString());

        // Save back
        await _redis.SetAsync(_recentArticlesKey, recent);
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
}