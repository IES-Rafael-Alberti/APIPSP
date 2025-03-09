using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoApi.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Add MongoDB connection string from environment variables
builder.Configuration["MONGO_URI"] = Environment.GetEnvironmentVariable("MONGO_URI");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the MongoDB repository
builder.Services.AddSingleton<ScoreRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();