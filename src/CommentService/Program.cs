using Shared;
using Microsoft.EntityFrameworkCore;
using Polly;
using CommentService.Data;
using CommentService.Interfaces;
using CommentService.Repositories;
using StackExchange.Redis;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
var dbName = builder.Configuration["DATABASE_NAME"] ?? "CommentDb";
// Allows the service to use its own database.
builder.Services.AddDbContext<CommentDbContext>(options => options.UseSqlServer($"Server=sqlserver,1433;Database={dbName};User Id=sa;Password={builder.Configuration["SA_PASSWORD"]};TrustServerCertificate=True"));

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect("redis:6379"));
builder.Services.AddSingleton<RedisHelper>();

// Prometheus metrics endpoint
builder.Services.AddSingleton<CollectorRegistry>(Metrics.DefaultRegistry);

// Adding CommentCache
builder.Services.AddSingleton<CommentCache>();

// Dependency Injection - Outcomment ICommentRepository when testing for mocking
// builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService.Services.CommentService>();

// Testing purposes
var mockData = new CommentMockData();
// Singletons purpose: Mock data lives for the lifetime of the application.
builder.Services.AddSingleton<ICommentRepository>(mockData.commentRepository);

// If ProfanityService fails, CommentService stops calling it temporarily, but can continue to store raw comments or provide fallback logic
// If 2 consecutive calls fail, the circuit breaker will stop calls for 15 seconds. 
// Then allow one call to test if the service is back up..otherwise the circuit breaker goes back to "open" state
builder.Services.AddHttpClient("Profanity", client =>
{
    client.BaseAddress = new Uri("http://profanity-service:80");
}).AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromSeconds(15)));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

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

app.Run();

