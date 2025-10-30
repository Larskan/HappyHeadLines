using Microsoft.AspNetCore.Mvc;
using SubscriberService.Interfaces;
using SubscriberService.Models;

namespace SubscriberService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriberController : ControllerBase
{
    private readonly ISubcriberService _service;

    // TODO: Add featurehub
    public SubscriberController(ISubcriberService service)
    {
        _service = service;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] Subscriber input, CancellationToken ct)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.Email))
            return BadRequest("Email required");

        //Insert checker if subscriber is enabled

        var created = await _service.AddSubscriberAsync(input, ct);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpDelete("unsubscribe/{id:int}")]
    public async Task<IActionResult> Unsubscribe(int id, CancellationToken ct)
    {
        var removed = await _service.RemoveSubscriberAsync(id, ct);
        if (!removed) return NotFound();
        return NoContent();
    }

    [HttpGet("subscribers")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var all = await _service.GetAllAsync(ct);
        return Ok(all);
    }
    
}