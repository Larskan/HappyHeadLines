using CommentService.Models;

namespace CommentService.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByCommentIdAsync(int id); //Find comment by its ID
    Task<List<Comment>> GetByArticleIdAsync(int articleId); //Find all comments for a specific article
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<bool> DeleteCommentAsync(int id);
}