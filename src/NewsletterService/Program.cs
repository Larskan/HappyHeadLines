using NewsletterService;
using NewsletterService.Data;
using NewsletterService.Interfaces;
using NewsletterService.Repositories;
using NewsletterService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);
// DB
builder.Services.AddDbContext<NewsletterDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NewsletterDb")));

// Repo
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();

// RabbitMQ singleton connection
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    return (IConnection)factory.CreateConnectionAsync();
});

// Background subscriber service
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<ArticleQueueSubscriber>();

var app = builder.Build();
app.Run();
