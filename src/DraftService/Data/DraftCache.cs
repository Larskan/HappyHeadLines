using DraftService.Interfaces;
using Shared;

namespace DraftService.Data;

public class DraftCache
{
    #region Global Variables
    private readonly IRedisHelper _redis;
    private readonly IDraftRepository _repo;
    private readonly string _recentDraftsKey = "recent_drafts"; // Key to track recent drafts
    private readonly int _maxDrafts = 20; //Only the 20 most recent drafts are cached
    private const string CacheKeyPrefix = "drafts:";
    #endregion Global Variables


    public DraftCache(IRedisHelper redis, IDraftRepository repo)
    {
        _repo = repo;
        _redis = redis;
    }

    public async Task<List<DraftDto>?> GetDraftAsync(int authorId)
    {
        string key = CacheKeyPrefix + authorId;

        // Check redis/cache first
        var cached = await _redis.GetAsync<List<DraftDto>>(key);
        if (cached != null)
        {
            await UpdateRecentDraftsAsync(authorId); // Marks this draft as recently used
            return cached;
        }

        // Cache miss: Fetch from DB
        var draft = await _repo.GetAllDraftsAsync(authorId);
        if (draft == null) return null;

        var dtoList = draft.Select(c => new DraftDto(c.Id, c.AuthorId, c.Title, c.Body, c.CreatedAt, c.UpdatedAt)).ToList();

        // Save to redis cache
        await _redis.SetAsync(key, dtoList);
        await UpdateRecentDraftsAsync(authorId); // Marks this draft as recently used

        // Trim LRU if needed, aka if we have more than 20 drafts in the cache, we trim the oldest ones away
        await TrimLRUAsync();
        return dtoList;
    }


    private async Task UpdateRecentDraftsAsync(int authorId)
    {
        // Fetch current recent list
        var recent = await _redis.GetAsync<List<string>>(_recentDraftsKey) ?? new List<string>();

        // Move or add draftId to the front
        recent.Remove(authorId.ToString()); // If this draft is already in cache, but lower down, we remove it first
        recent.Insert(0, authorId.ToString()); // Then we add the draft to the front, so it's the most recently used

        // Save back to Redis
        await _redis.SetAsync(_recentDraftsKey, recent); // No expiration, we want to keep track of recent drafts comments indefinitely.. or until we reach max capacity.
    }

    private async Task TrimLRUAsync()
    {
        var recent = await _redis.GetAsync<List<string>>(_recentDraftsKey) ?? new List<string>();
        while (recent.Count > _maxDrafts)
        {
            // Remove the least recently viewed drafts
            var lru = recent.Last();
            recent.RemoveAt(recent.Count - 1);

            // Remove its drafts from Redis
            await _redis.RemoveAsync($"drafts:{lru}");
        }

        // Save updated list
        await _redis.SetAsync(_recentDraftsKey, recent);

    }
}

