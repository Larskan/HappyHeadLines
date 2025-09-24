using CommentService.Models;
using CommentService.Data;
using CommentService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly CommentDbContext _dbContext;

    public CommentRepository(CommentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> GetByCommentIdAsync(Guid id) =>
        await _dbContext.Comments.FindAsync(id);

    public async Task<List<Comment>> GetByArticleIdAsync(Guid articleId) =>
        await _dbContext.Comments.Where(c => c.ArticleId == articleId).ToListAsync();

    public async Task<bool> DeleteCommentAsync(Guid id)
    {
        var comment = await _dbContext.Comments.FindAsync(id);
        if (comment is null) return false;
        _dbContext.Comments.Remove(comment);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}