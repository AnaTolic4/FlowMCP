var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer().WithHttpTransport();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapMcp("/mcp");
app.MapHealthChecks("/health/live");

app.Run();
