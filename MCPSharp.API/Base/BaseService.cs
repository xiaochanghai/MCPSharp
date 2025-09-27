using MCPSharp.API.Attributes;
using MCPSharp.API.Models.Mcp;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;

namespace MCPSharp.API.Interfaces;

public class BaseService<TService> : IBaseService where TService : class
{
    private readonly ILogger<BaseService<TService>> _logger;
    private readonly Dictionary<string, MethodInfo> _toolMethods;
    private readonly Lazy<TService> _serviceInstance;

    public BaseService(ILogger<BaseService<TService>> logger)
    {
        _logger = logger;
        _toolMethods = new Dictionary<string, MethodInfo>();

        _serviceInstance = new Lazy<TService>(() =>
        {
            if (this is TService service)
            {
                DiscoverMcpMethods();
                return service;
            }
            throw new InvalidOperationException($"Service must implement {typeof(TService).Name}");
        });
    }

    protected TService ServiceInstance => _serviceInstance.Value;

    private void DiscoverMcpMethods()
    {
        var type = typeof(TService);

        // Discover tool methods
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var toolAttr = method.GetCustomAttribute<McpToolAttribute>();
            if (toolAttr != null)
            {
                var name = string.IsNullOrEmpty(toolAttr.Name) ? method.Name : toolAttr.Name;
                _toolMethods[name] = method;
                _logger.LogInformation("Discovered tool: {ToolName}", name);
            }
        }
    }

    public virtual object HandleInitialize(JsonElement? parameters)
    {
        _logger.LogInformation("MCP server initialized");
        return new
        {
            protocolVersion = "2024-11-05",
            capabilities = new
            {
                tools = new { }
            },
            serverInfo = new McpServerInfo
            {
                Name = "EU.Core FastMCP Server",
                Version = "1.0.0"
            }
        };
    }

    public virtual object GetAvailableTools()
    {
        var allTools = GetTools().ToArray();

        _logger.LogInformation($"Returning {allTools.Length} available tools");
        return new { tools = allTools };
    }

    public virtual async Task<McpToolResult> HandleToolCallAsync(JsonElement? parameters)
    {
        if (parameters == null)
            throw new ArgumentException("Missing parameters");

        var toolName = parameters.Value.GetProperty("name").GetString();
        var arguments = parameters.Value.TryGetProperty("arguments", out var args) ? args : default;

        if (string.IsNullOrEmpty(toolName))
            throw new ArgumentException("Tool name is required");

        _logger.LogInformation($"Executing tool: {toolName}");

        // Find the service that can handle this tool
        var isExist = CanHandle(toolName);

        if (!isExist)
            throw new ArgumentException($"No service found for tool: {toolName}");
        var dynamicObject = ConvertToDynamic(arguments);
        return await ExecuteToolAsync(toolName, arguments, dynamicObject);
    }

    public static dynamic? ConvertToDynamic(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                // For objects, create an ExpandoObject
                dynamic expando = new ExpandoObject();
                var expandoDict = (IDictionary<string, object?>)expando;
                foreach (var property in element.EnumerateObject())
                {
                    // Recursively convert property values and add to dictionary
                    expandoDict[property.Name] = ConvertToDynamic(property.Value);
                }
                return expando;

            case JsonValueKind.Array:
                // For arrays, create a List<dynamic>
                var list = new List<dynamic?>();
                foreach (var item in element.EnumerateArray())
                {
                    // Recursively convert array items and add to list
                    list.Add(ConvertToDynamic(item));
                }
                return list;

            // For primitive types, return corresponding C# value directly
            case JsonValueKind.String:
                return element.GetString();

            case JsonValueKind.Number:
                // Try to get number with different precision
                if (element.TryGetInt32(out int intValue)) return intValue;
                if (element.TryGetInt64(out long longValue)) return longValue;
                return element.GetDouble(); // Default to double

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;

            default:
                // For any other type, return raw text
                return element.GetRawText();
        }
    }

    public virtual IEnumerable<McpTool> GetTools()
    {
        _ = ServiceInstance;

        var tools = new List<McpTool>();

        // Dynamically generate tool list
        foreach (var kvp in _toolMethods)
        {
            var method = kvp.Value;
            var toolAttr = method.GetCustomAttribute<McpToolAttribute>();

            tools.Add(new McpTool
            {
                Name = kvp.Key,
                Description = toolAttr?.Description ?? $"Tool: {method.Name}",
                InputSchema = toolAttr?.InputSchema ?? new
                {
                    type = "object",
                    properties = new
                    {
                    }
                }
            });
        }

        return tools;
    }

    public bool CanHandle(string toolName)
    {
        _ = ServiceInstance;
        return _toolMethods.ContainsKey(toolName);
    }

    public virtual async Task<McpToolResult> ExecuteToolAsync(string toolName, JsonElement arguments, dynamic? dynamicObject)
    {
        _logger.LogInformation($"Executing tool: {toolName}");

        if (!_toolMethods.TryGetValue(toolName, out var method))
        {
            throw new ArgumentException($"Unknown tool: {toolName}");
        }

        try
        {
            // Dynamically invoke service method
            var result = method.Invoke(ServiceInstance, [dynamicObject]);

            // If method is async, await completion
            if (result is Task task)
            {
                await task;

                // Get Task result
                var resultProperty = task.GetType().GetProperty("Result");
                if (resultProperty != null)
                {
                    result = resultProperty.GetValue(task);
                }
            }

            return result as McpToolResult ?? throw new InvalidOperationException($"Method {toolName} did not return McpToolResult");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error executing tool {toolName}");
            throw new InvalidOperationException($"Error executing tool {toolName}: {ex.Message}", ex);
        }
    }
}