using Shared;
using Microsoft.EntityFrameworkCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CommentDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient("Profanity", client =>
{
    client.BaseAddress = new Uri("http://profanity-service:80");
});

// If ProfanityService fails, CommentService stops calling it temporarily, but can continue to store raw comments or provide fallback logic
builder.Services.AddHttpClient("Profanity", client =>
{
    client.BaseAddress = new Uri("http://profanity-service:80");
}).AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromSeconds(15)));
var app = builder.Build();

app.MapPost("/articles/{articleId}/comments", async (int articleId, CommentDto dto, CommentDb db, IHttpClientFactory httpClientFactory) =>
{

    var client = httpClientFactory.CreateClient("Profanity");
    string filteredText = dto.Body;
    try
    {
        var response = await client.PostAsJsonAsync("/filter", new { text = dto.Body });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ProfanityDto>();
            filteredText = result!.Filtered;
        }
    }
    catch (Exception ex)
    {
        // Log exception but continue
        Console.WriteLine($"Exception calling profanity service: {ex.Message}");
    }

    var comment = new Comment { ArticleId = articleId, Text = filteredText };
    db.Comments.Add(comment);
    await db.SaveChangesAsync();
    return Results.Created($"/articles/{articleId}/comments/{comment.Id}", comment);
});

app.MapGet("/articles/{articleId}/comments", async (int articleId, CommentDb db) =>
{
    return await db.Comments.Where(c => c.ArticleId == articleId).ToListAsync();
});

app.Run();

// Note: Should call ProfanityService /filter to filter comments before saving to DB
// Note: Stores commentDatabase via EF Core
// Note: Expose GET /Articles/{id}/comments to get comments for an article
