using System.Text.Json.Serialization;

namespace MCPSharp.API.Models.Mcp; 

public class McpToolResult
{
    [JsonPropertyName("content")]
    public McpContent[] Content { get; set; } = Array.Empty<McpContent>();
}