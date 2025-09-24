using ArticleService.Services;
using ArticleService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.IO.Compression;

namespace ArticleService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _service;

    public ArticleController(IArticleService service)
    {
        _service = service;
    }

    private string GetContinent() =>
        HttpContext.Request.Headers["X-Continent"].ToString() ?? "Global";

    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var article = await _service.GetByIdAsync(id, GetContinent());
        return article is null ? NotFound() : Ok(article);
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var articles = await _service.GetAllAsync(GetContinent());
        return Ok(articles);
    }
  

    [HttpPost]
    public async Task<ActionResult> CreateArticle([FromBody] ArticleDto articleDto)
    {
        var created = await _service.CreateArticleAsync(articleDto, GetContinent());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] ArticleDto articleDto)
    {
        var updated = await _service.UpdateArticleAsync(id, articleDto, GetContinent());
        return updated ? Ok(articleDto) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        var deleted = await _service.DeleteArticleAsync(id, GetContinent());
        return deleted ? NoContent() : NotFound();
    }
}