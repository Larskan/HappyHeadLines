using ArticleService.Models;
using ArticleService.Helpers;
using Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// CREATE
app.MapPost("/articles", async (ArticleDto dto, HttpContext ctx) =>
{
    var continent = ctx.Request.Headers["X-Continent"].ToString();
    // Select the appropriate database based on the continent header
    using var db = DatabaseSelector.GetDbContext(app.Services, continent);
    var article = new Article
    {
        Id = Guid.NewGuid(),
        Title = dto.Title,
        Body = dto.Body,
        Author = dto.Author,
        PublishedAt = dto.PublishedAt
    };
    db.Articles.Add(article);
    await db.SaveChangesAsync();
    return Results.Created($"/articles/{article.Id}", article);
});

// READ
app.MapGet("/articles/{id}", async (Guid id, HttpContext ctx) =>
{
    var continent = ctx.Request.Headers["X-Continent"].ToString();
    // Select the appropriate database based on the continent header
    using var db = DatabaseSelector.GetDbContext(app.Services, continent);
    var article = await db.Articles.FindAsync(id);
    return article is not null ? Results.Ok(article) : Results.NotFound();
});

// UPDATE
app.MapPut("/articles/{id}", async (Guid id, ArticleDto dto, HttpContext ctx) =>
{
    var continent = ctx.Request.Headers["X-Continent"].ToString();
    // Select the appropriate database based on the continent header
    using var db = DatabaseSelector.GetDbContext(app.Services, continent);
    var article = await db.Articles.FindAsync(id);

    if (article is null) return Results.NotFound(); // 404 if not found

    article.Title = dto.Title;
    article.Body = dto.Body;
    article.Author = dto.Author;
    article.PublishedAt = dto.PublishedAt;
    article.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(article);
});

// DELETE
app.MapDelete("/articles/{id}", async (Guid id, HttpContext ctx) =>
{
    var continent = ctx.Request.Headers["X-Continent"].ToString();
    // Select the appropriate database based on the continent header
    using var db = DatabaseSelector.GetDbContext(app.Services, continent);
    var article = await db.Articles.FindAsync(id);

    if (article is null) return Results.NotFound(); // 404 if not found

    db.Articles.Remove(article);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// List all articles
app.MapGet("/articles", async (HttpContext ctx) =>
{
    var continent = ctx.Request.Headers["X-Continent"].ToString();
    using var db = DatabaseSelector.GetDbContext(app.Services, continent);
    var articles = await db.Articles.ToListAsync();
    return Results.Ok(articles);
});

app.Run();