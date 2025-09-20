using System.Text.Json.Serialization;

namespace MCPSharp.API.Models.Mcp;

public class JsonRpcError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}