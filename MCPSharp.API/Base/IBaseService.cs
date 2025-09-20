using MCPSharp.API.Models.Mcp;
using System.Text.Json;

namespace MCPSharp.API.Interfaces;

public interface IBaseService
{
    /// <summary>
    /// Handle MCP initialization
    /// </summary>
    object HandleInitialize(JsonElement? parameters);
    
    /// <summary>
    /// Get all available tools
    /// </summary>
    object GetAvailableTools();
    
    /// <summary>
    /// Execute a tool call
    /// </summary>
    Task<McpToolResult> HandleToolCallAsync(JsonElement? parameters);
}