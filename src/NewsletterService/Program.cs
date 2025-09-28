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

// Background subscriber service
builder.Services.AddHostedService<ArticleQueueSubscriber>();

var app = builder.Build();
app.Run();
