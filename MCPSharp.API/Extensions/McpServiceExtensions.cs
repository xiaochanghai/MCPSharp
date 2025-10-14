using MCPSharp.API.Interfaces;
using MCPSharp.API.Services;

namespace MCPSharp.API.Extensions;

/// <summary>
/// ServiceExtension
/// </summary>
public static class McpServiceExtensions
{
    /// <summary>
    /// Adds MCP services to the service collection.
    /// </summary>
    public static IServiceCollection AddMcpServices(this IServiceCollection services)
    {
        services.AddScoped<ITestService, TestService>(); // Register service

        return services;
    }
}