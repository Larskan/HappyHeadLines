using ArticleService.Repositories;
using ArticleService.Interfaces;
using StackExchange.Redis;
using Shared;
using ArticleService.Helpers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection - Outcomment IArticleRepository when testing for mocking
// builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService.Services.ArticleService>();

// Testing purposes
var mockData = new ArticleMockData();
// Singletons purpose: Mock data lives for the lifetime of the application.
builder.Services.AddSingleton<IArticleRepository>(mockData.articleRepository);

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect("redis:6379"));
builder.Services.AddSingleton<RedisHelper>();
builder.Services.AddSingleton<ArticleCache>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ArticleCache>());

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();