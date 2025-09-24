using CommentService.Interfaces;
using Shared;
using Microsoft.AspNetCore.Mvc;
using CommentService.Models;

namespace CommentService.Controllers;

[ApiController]
[Route("api/articles/[articleId]/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetByArticleIdAsync(Guid articleId)
    {
        var comments = await _service.GetByArticleIdAsync(articleId);
        return Ok(comments);
    }

    [HttpGet("{articleId:guid}")]
    public async Task<ActionResult<CommentDto>> GetByIdAsync(Guid articleId)
    {
        var comments = await _service.GetByArticleIdAsync(articleId);
        return comments is null ? NotFound() : Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommentAsync(Guid articleId, [FromBody] CommentDto commentDto)
    {
        var created = await _service.CreateCommentAsync(articleId, commentDto);
        return CreatedAtAction(nameof(GetByArticleIdAsync), new { articleId, id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCommentAsync(Guid id)
    {
        var deleted = await _service.DeleteCommentAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}