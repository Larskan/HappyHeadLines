using NewsletterService;
using NewsletterService.Data;
using NewsletterService.Interfaces;
using NewsletterService.Repositories;
using NewsletterService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Builder;
using Shared;


var builder = WebApplication.CreateBuilder(args);

// Central logging and tracing
builder.Host.UseCentralLogging("NewsletterService");
builder.Services.AddCentralTracing("NewsletterService");

// Controllers and swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// DB
builder.Services.AddDbContext<NewsletterDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NewsletterDb")));

// RabbitMQ singleton connection
builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory() { HostName = "rabbitmq" };
    return (IConnection)factory.CreateConnectionAsync();
});

// Dependency Injection
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddSingleton<IArticleQueuePublisher, ArticleQueue>();
builder.Services.AddSingleton<IArticleQueueSubscriber, ArticleQueue>();

// Background subscriber service
builder.Services.AddHostedService<NewsletterArticleSubscriber>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
