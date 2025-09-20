using System.Text.Json.Serialization;

namespace MCPSharp.API.Models.Mcp;

public class McpContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";
    
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}