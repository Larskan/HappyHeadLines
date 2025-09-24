using ArticleService.Repositories;
using ArticleService.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService.Services.ArticleService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();