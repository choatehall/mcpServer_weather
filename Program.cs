using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using System.Net.Http.Headers;

// When creating the ApplicationHostBuilder, ensure you use CreateEmptyApplicationBuilder
// instead of CreateDefaultBuilder. This ensures that the server does not write any additional messages
// to the console. This is only neccessary for servers using STDIO transport.
var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddSingleton(_ =>
{
    var client = new HttpClient() { BaseAddress = new Uri("https://api.weather.gov") };
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-tool", "1.0"));
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/geo+json"));
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    
    // Log the client configuration
    Console.Error.WriteLine($"DEBUG: HTTP Client BaseAddress: {client.BaseAddress}");
    Console.Error.WriteLine($"DEBUG: HTTP Client User-Agent: {client.DefaultRequestHeaders.UserAgent}");
    Console.Error.WriteLine($"DEBUG: HTTP Client Accept: {client.DefaultRequestHeaders.Accept}");
    
    return client;
});

var app = builder.Build();

await app.RunAsync();