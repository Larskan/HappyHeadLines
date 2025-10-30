using Microsoft.EntityFrameworkCore;
using SubscriberService.Data;
using SubscriberService.Interfaces;
using SubscriberService.Repositories;
using SubscriberService.Services;
using Shared;


var builder = WebApplication.CreateBuilder(args);

// Services to DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
builder.Services.AddDbContext<SubscriberDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SubscriberDB")));

// Rabbit
builder.Services.AddSingleton<IRabbitConnectionProvider, RabbitConnectionProvider>();

// SubscriberQueue for publishing Subscriptions
builder.Services.AddSingleton<ISubscriberQueuePublisher, SubscriberQueue>();
builder.Services.AddSingleton<ISubscriberQueueSubscriber, SubscriberQueue>();

// Repo, Service
builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();
builder.Services.AddScoped<ISubcriberService, SubscriberService.Services.SubscriberService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
