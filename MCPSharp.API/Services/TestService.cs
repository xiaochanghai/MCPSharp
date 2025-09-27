using MCPSharp.API.Attributes;
using MCPSharp.API.Interfaces;
using MCPSharp.API.Models.Mcp;
using System.ComponentModel;


namespace MCPSharp.API.Services;

public class InputSchemaArguments()
{
    /// <summary>
    /// Name
    /// </summary>
    [Description("Name")]
    public string? name { get; set; }

}

public class TestService : BaseService<TestService>, ITestService
{
    public TestService(ILogger<TestService> logger) : base(logger)
    { 
    }

    [McpTool("test_hello", "A simple test supplier tool that says hello", typeof(InputSchemaArguments))]
    public async Task<McpToolResult> HandleTestHello(object arguments)
    {
        var aaqq = JsonHelper.JsonToObj<InputSchemaArguments>(JsonHelper.ObjToJson(arguments));

        //if (arguments.ValueKind != JsonValueKind.Undefined &&
        //    arguments.TryGetProperty("name", out var nameProperty))
        //{
        //    name = nameProperty.GetString() ?? "World";
        //}

        return new McpToolResult
        {
            Content = new[]
            {
                new McpContent
                {
                    Type = "text",
                    Text = $"Hello, {aaqq.name},{DateTime.Now}! MCP server is working! "
                }
            }
        };
    }

}