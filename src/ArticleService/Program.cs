using ArticleService.Repositories;
using ArticleService.Interfaces;
using StackExchange.Redis;
using Shared;
using ArticleService.Helpers;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection - Outcomment IArticleRepository when testing for mocking
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService.Services.ArticleService>();

// Testing purposes
// var mockData = new ArticleMockData();
// // Singletons purpose: Mock data lives for the lifetime of the application.
// builder.Services.AddSingleton<IArticleRepository>(mockData.articleRepository);

// Prometheus metrics endpoint
builder.Services.AddSingleton<CollectorRegistry>(Metrics.DefaultRegistry);

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect("redis:6379"));
builder.Services.AddSingleton<IRedisHelper, RedisHelper>();
builder.Services.AddSingleton<ArticleCache>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ArticleCache>());

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// The multiple databases of Articles.
var continents = new[] { "NorthAmerica", "SouthAmerica", "Europe", "Africa", "Asia", "Australia", "Antarctica", "Global" };
foreach (var continent in continents)
{
    var context = DatabaseSelector.GetDbContext(app.Services, continent);
    context.Database.EnsureCreated();
}

// Expose metrics for prometheus scraping
app.UseMetricServer();
app.UseHttpMetrics();
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
    _ = endpoints.MapMetrics(); // Map Prometheus metrics endpoint
});

app.MapControllers();

app.Run();