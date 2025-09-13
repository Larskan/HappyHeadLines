var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

// Note: Should call ProfanityService /filter to filter comments before saving to DB
// Note: Stores commentDatabase via EF Core
// Note: Expose GET /Articles/{id}/comments to get comments for an article