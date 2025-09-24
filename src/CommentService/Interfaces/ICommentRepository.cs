using CommentService.Models;

namespace CommentService.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByCommentIdAsync(Guid id); //Find comment by its ID
    Task<List<Comment>> GetByArticleIdAsync(Guid articleId); //Find all comments for a specific article
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<bool> DeleteCommentAsync(Guid id);
}