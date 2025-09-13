using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProfanityDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/profanities", async (ProfanityDb db) => await db.BlockedWords.OrderBy(w => w.Word).Select(w => w.Word).ToListAsync());
app.MapPost("/profanity", async (ProfanityDb db, ProfanityWordDto dto) =>
{
    var w = new BlockedWord { Word = dto.Word.ToLowerInvariant() };
    db.BlockedWords.Add(w);
    await db.SaveChangesAsync();
    return Results.Created($"/profanity/{w.Id}", w);
});

app.MapPost("/filter", async (ProfanityDb db, FilterDto dto) =>
{
    var blocked = await db.BlockedWords.Select(w => w.Word).ToListAsync();
    var body = dto.Text;
    foreach (var b in blocked)
        body = System.Text.RegularExpressions.Regex.Replace(body, @"\b" + Regex.Escape(b) + @"\b", new string('*', b.Length), RegexOptions.IgnoreCase);
    return Results.Ok(new { filtered = body });
});

app.Run();

public record ProfanityWordDto(string Word);
public record FilterDto(string Text);

public class ProfanityDb : DbContext
{
    public ProfanityDb(DbContextOptions<ProfanityDb> options) : base(options) { }
    public DbSet<BlockedWord> BlockedWords { get; set; } = null!;
}

public class BlockedWord
{
    public int Id { get; set; }
    public string Word { get; set; } = null!;
}
