using AiSync.MCP.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register HttpClient pointing to the AI-Sync backend REST API
builder.Services.AddHttpClient("AiSyncApi", client =>
{
    var baseUrl = builder.Configuration["AiSyncApi:BaseUrl"]
                  ?? "http://localhost:5213";
    client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register MCP server over STDIO and discover all [McpServerToolType] classes
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
