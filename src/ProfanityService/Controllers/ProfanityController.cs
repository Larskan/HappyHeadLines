using Microsoft.AspNetCore.Mvc;
using ProfanityService.Services;
using ProfanityService.Interfaces;

namespace ProfanityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfanityController : ControllerBase
{
    private readonly IProfanityService _profanityService;

    public ProfanityController(IProfanityService profanityService)
    {
        _profanityService = profanityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBlockedWords()
    {
        var blockedWords = await _profanityService.GetAllBlockedWordsAsync();
        return Ok(blockedWords);
    }

    [HttpPost]
    public async Task<IActionResult> AddBlockedWord([FromBody] string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return BadRequest("Word cannot be empty.");

        var addedWord = await _profanityService.AddBlockedWordAsync(word);
        return CreatedAtAction(nameof(GetAllBlockedWords), new { id = addedWord.Id }, addedWord);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> FilterProfanity([FromBody] string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return BadRequest("Text cannot be empty.");

        var filteredText = await _profanityService.FilterProfanityAsync(text);
        return Ok(new { filteredText });
    }
}