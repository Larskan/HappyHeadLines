using Serilog;
using Microsoft.EntityFrameworkCore;
using Shared;
using DraftService.Interfaces;
using DraftService.Services;


var builder = WebApplication.CreateBuilder(args);

// Logging attempt
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("logs/draftservice-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext());

// DB
var dbName = builder.Configuration["DATABASE_NAME"] ?? "DraftDb";
// Allows the service to use its own database.
builder.Services.AddDbContext<DraftService.Data.DraftDbContext>(options => options.UseSqlServer($"Server=sqlserver,1433;Database={dbName};User Id=sa;Password={builder.Configuration["SA_PASSWORD"]};"));

// Dependency Injection
builder.Services.AddScoped<IDraftService, DraftService.Services.DraftService>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
