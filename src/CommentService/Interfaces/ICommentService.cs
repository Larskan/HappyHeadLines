using CommentService.Models;
using CommentService.Data;
using CommentService.Repositories;
using Shared;

namespace CommentService.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetByArticleIdAsync(Guid articleId);
    Task<CommentDto> CreateCommentAsync(Guid articleId, CommentDto commentDto);
    Task<bool> DeleteCommentAsync(Guid id);
}