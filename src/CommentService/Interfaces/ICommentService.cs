using CommentService.Models;
using CommentService.Data;
using CommentService.Repositories;
using Shared;

namespace CommentService.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetByArticleIdAsync(int articleId);
    Task<CommentDto> CreateCommentAsync(int articleId, CommentDto commentDto);
    Task<bool> DeleteCommentAsync(int id);
}