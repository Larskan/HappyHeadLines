using NewsletterService;
using NewsletterService.Data;
using NewsletterService.Interfaces;
using NewsletterService.Repositories;
using NewsletterService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Shared;
using Polly;
using Polly.Extensions.Http;



var builder = WebApplication.CreateBuilder(args);

// Central logging and tracing
builder.Host.UseCentralLogging("NewsletterService");
builder.Services.AddCentralTracing("NewsletterService");

// Controllers and swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Polly
var subscriberServiceBaseUrl = builder.Configuration["SubscriberService:BaseUrl"] ?? "http://subscriber-service:8080"; // default fallback
builder.Services.AddHttpClient<INewsletterSubscriberRepository, NewsletterSubscriberRepository>(client =>
{
    client.BaseAddress = new Uri(subscriberServiceBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler(ResiliencePolicy.GetRetryPolicy())
.AddPolicyHandler(ResiliencePolicy.GetCircuitBreakerPolicy());

// DB
builder.Services.AddDbContext<NewsletterDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NewsletterDb")));

// RabbitMQ singleton connection
builder.Services.AddSingleton<IRabbitConnectionProvider, RabbitConnectionProvider>();

// Shared queue binding for subscribers, consumes subscriber messages
builder.Services.AddSingleton<ISubscriberQueueSubscriber, SubscriberQueue>();

// Dependency Injection
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddSingleton<IArticleQueuePublisher, ArticleQueue>();
builder.Services.AddSingleton<IArticleQueueSubscriber, ArticleQueue>();


// Background subscriber service
builder.Services.AddHostedService<NewsletterArticleSubscriber>();
builder.Services.AddHostedService<SubscriptionEventProcessor>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
