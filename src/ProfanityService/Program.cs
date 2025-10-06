using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;
using ProfanityService.Interfaces;
using ProfanityService.Repositories;
using ProfanityService.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
var dbName = builder.Configuration["DATABASE_NAME"] ?? "ProfanityDb";
// var connectionstring = "Server=localhost,1435;Database=ProfanityDB;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;Encrypt=False;";
// builder.Services.AddDbContext<ProfanityDbContext>(options => options.UseSqlServer(connectionstring));
// Allows the service to use its own database.
builder.Services.AddDbContext<ProfanityDbContext>(options => options.UseSqlServer($"Server=sqlserver,1433;Database={dbName};User Id=sa;Password={builder.Configuration["SA_PASSWORD"]};TrustServerCertificate=True;"));

// Dependency Injection
builder.Services.AddScoped<IProfanityRepository, ProfanityRepository>();
builder.Services.AddScoped<IProfanityService, ProfanityService.Services.ProfanityService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();


app.Run();
