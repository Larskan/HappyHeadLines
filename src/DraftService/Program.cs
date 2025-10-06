using Microsoft.EntityFrameworkCore;
using DraftService.Interfaces;
using DraftService.Data;
using DraftService.Services;
using DraftService.Repositories;
using Shared;



var builder = WebApplication.CreateBuilder(args);

// Central logging and tracing
builder.Host.UseCentralLogging("DraftService");
builder.Services.AddCentralTracing("DraftService");

// Controllers and swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
var dbName = builder.Configuration["DATABASE_NAME"] ?? "DraftDb";
// Allows the service to use its own database.
builder.Services.AddDbContext<DraftDbContext>(options => options.UseSqlServer($"Server=sqlserver,1433;Database={dbName};User Id=sa;Password={builder.Configuration["SA_PASSWORD"]};TrustServerCertificate=True"));

// Dependency Injection
builder.Services.AddScoped<IDraftService, DraftService.Services.DraftService>();
builder.Services.AddScoped<IDraftRepository, DraftRepository>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
