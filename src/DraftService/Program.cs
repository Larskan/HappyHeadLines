using Serilog;
using Microsoft.EntityFrameworkCore;
using DraftService.Interfaces;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Logging attempt
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("logs/draftservice-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext());

// DB
var dbName = builder.Configuration["DATABASE_NAME"] ?? "DraftDb";
// Allows the service to use its own database.
builder.Services.AddDbContext<DraftService.Data.DraftDbContext>(options => options.UseSqlServer($"Server=sqlserver,1433;Database={dbName};User Id=sa;Password={builder.Configuration["SA_PASSWORD"]};TrustServerCertificate=True"));

// Dependency Injection
builder.Services.AddScoped<IDraftService, DraftService.Services.DraftService>();
builder.Services.AddScoped<IDraftRepository, DraftService.Repositories.DraftRepository>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
