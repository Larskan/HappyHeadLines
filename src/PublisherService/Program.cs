using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using System.Text;
using System.Text.Json;
using Shared;
using RabbitMQ.Client;
using PublisherService.Data;
using Microsoft.EntityFrameworkCore;
using PublisherService.Interfaces;
using PublisherService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Central logging and tracing
builder.Host.UseCentralLogging("PublisherService");
builder.Services.AddCentralTracing("PublisherService");

// Controllers and swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
var dbName = builder.Configuration["DATABASE_NAME"] ?? "PublisherDb";
builder.Services.AddDbContext<PublisherDbContext>(options => options.UseSqlServer($"Server=sqlserver,1433;Database={dbName};User Id=sa;Password={builder.Configuration["SA_PASSWORD"]};TrustServerCertificate=True"));

// Rabbitmq connection singleton
builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory { HostName = "rabbitmq" };
    return (IConnection)factory.CreateConnectionAsync();
});

//Dependency Injection
builder.Services.AddScoped<IPublishArticleRepository, PublishArticleRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();


app.Run();


