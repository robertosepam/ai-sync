using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AiSync.Application;
using AiSync.Application.Interfaces;
using AiSync.Infrastructure;
using AiSync.McpServer.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<McpToolServer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<McpToolServer>());

var app = builder.Build();

await app.RunAsync();
