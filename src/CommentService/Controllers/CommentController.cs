using CommentService.Interfaces;
using Shared;
using Microsoft.AspNetCore.Mvc;
using CommentService.Models;

namespace CommentService.Controllers;

[ApiController]
[Route("api/articles/{articleId}/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetByArticleIdAsync(int articleId)
    {
        var comments = await _service.GetByArticleIdAsync(articleId);
        return Ok(comments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetByIdAsync(int articleId, int id)
    {
        var comments = await _service.GetByArticleIdAsync(id);
        return comments is null ? NotFound() : Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommentAsync(int articleId, [FromBody] CommentDto commentDto)
    {
        var created = await _service.CreateCommentAsync(articleId, commentDto);
        return CreatedAtAction(nameof(GetByArticleIdAsync), new { articleId, id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCommentAsync(int articleId, int id)
    {
        var deleted = await _service.DeleteCommentAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}