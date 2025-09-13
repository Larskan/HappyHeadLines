using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using System.Text;
using System.Text.Json;
using Shared;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var app = builder.Build();

app.MapPost("/publish", async (PublishRequest request) =>
{
    // 1) Call ProfanityService filter endpoint
    var http = new HttpClient { BaseAddress = new Uri(builder.Configuration["PROFANITY_URL"] ?? "http://localhost:5001") };
    var filterResponse = await http.PostAsJsonAsync("/filter", new { Text = request.Body });
    var filtered = await filterResponse.Content.ReadFromJsonAsync<FilteredResponse>();

    // 2) Create ArticleDto
    var article = new ArticleDto(Guid.NewGuid(), request.Title, filtered!.Filtered, DateTime.UtcNow, request.Author);

    // 3) Publish to RabbitMQ (async API in RabbitMQ.Client 7.x)
    var exchange = "happy.exchange";
    var rabbitHost = builder.Configuration["RABBIT_HOST"] ?? "localhost";

    await using var connection = await RabbitHelper.CreateConnectionAsync(rabbitHost);
    await using var channel = await connection.CreateChannelAsync();
    await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true);
    await RabbitHelper.PublishAsync(channel, exchange, "", article);
    return Results.Ok(article);
});

app.Run();

record PublishRequest(string Title, string Body, string Author);
record FilteredResponse(string Filtered);
