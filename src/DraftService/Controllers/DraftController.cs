using DraftService.Interfaces;
using Shared;
using Microsoft.AspNetCore.Mvc;

namespace DraftService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftController : ControllerBase
{
    private readonly IDraftService _draftService;

    public DraftController(IDraftService draftService)
    {
        _draftService = draftService;
    }

    // GET: api/Draft
    [HttpGet]
    public async Task<ActionResult<List<DraftDto>>> GetAllDrafts([FromQuery] Guid authorId)
    {
        var drafts = await _draftService.GetAllDraftsAsync(authorId);
        return Ok(drafts);
    }

    // GET: api/Draft/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DraftDto>> GetDraftById(Guid id)
    {
        var draft = await _draftService.GetDraftByIdAsync(id);
        if (draft is null) return NotFound();
        return Ok(draft);
    }

    // POST: api/Draft
    [HttpPost]
    public async Task<ActionResult<DraftCreateDto>> CreateDraft([FromBody] DraftCreateDto draftCreateDto)
    {
        var createdDraft = await _draftService.CreateDraftAsync(draftCreateDto);
        return CreatedAtAction(nameof(GetDraftById), new { id = createdDraft.AuthorId }, createdDraft);
    }

    // PUT: api/Draft/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDraft(Guid id, [FromBody] DraftUpdateDto draftUpdateDto)
    {
        var updated = await _draftService.UpdateDraftAsync(id, draftUpdateDto);
        if (!updated) return NotFound();
        return NoContent();
    }

    // DELETE: api/Draft/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDraft(Guid id)
    {
        var deleted = await _draftService.DeleteDraftAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}