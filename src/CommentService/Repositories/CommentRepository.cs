using CommentService.Models;
using CommentService.Data;
using CommentService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace CommentService.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly CommentDbContext _dbContext;
    private readonly RedisHelper _redis;

    public CommentRepository(CommentDbContext dbContext, RedisHelper redis)
    {
        _dbContext = dbContext;
        _redis = redis;
    }

    // Redis first, then DB if cache miss. 
    // Caching logic isolated in Repo.
    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();

        // Update cache if already exists
        var key = $"comments:{comment.ArticleId}";
        var cached = await _redis.GetAsync<List<Comment>>(key);
        if (cached != null)
        {
            cached.Add(comment);
            // The days are just a random number, we got auto clean up through docker compose.
            await _redis.SetAsync(key, cached, TimeSpan.FromDays(7));
        }
        return comment;
    }

    public async Task<Comment?> GetByCommentIdAsync(Guid id) =>
        await _dbContext.Comments.FindAsync(id);

    public async Task<List<Comment>> GetByArticleIdAsync(Guid articleId)
    {
        var key = $"comments:{articleId}";
        var cached = await _redis.GetAsync<List<Comment>>(key);
        if (cached != null) return cached;

        var comments = await _dbContext.Comments.Where(c => c.ArticleId == articleId).ToListAsync();
        await _redis.SetAsync(key, comments, TimeSpan.FromDays(7));
        return comments;
    }


    public async Task<bool> DeleteCommentAsync(Guid id)
    {
        var comment = await _dbContext.Comments.FindAsync(id);
        if (comment is null) return false;
        _dbContext.Comments.Remove(comment);
        var success = await _dbContext.SaveChangesAsync() > 0;
        if (success)
        {
            var key = $"comments:{comment.ArticleId}";
            var cached = await _redis.GetAsync<List<Comment>>(key);
            if (cached != null)
            {
                cached.RemoveAll(c => c.Id == id);
                await _redis.SetAsync(key, cached, TimeSpan.FromDays(7));
            }
        }
        return success;
    }
}