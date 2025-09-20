# MCPSharp

English | [中文](README.md)

A Model Context Protocol (MCP) server and client framework based on .NET 8, supporting interaction between AI assistants and external tools.

## 🚀 Project Introduction

MCPSharp is a complete MCP protocol implementation designed to provide high-performance, extensible network communication capabilities for various .NET development scenarios. The project includes:

- **MCPSharp.API**: MCP server providing tool registration and JSON-RPC interfaces
- **MCPSharp.Client**: MCP client supporting AI model integration and tool invocation

## 📁 Project Structure

```markdown:README.en.md
MCPSharp/
├── MCPSharp.API/                 # MCP Server
│   ├── Controllers/              # API Controllers
│   │   └── TestController.cs     # Test Controller
│   ├── Services/                 # Business Services
│   │   └── TestService.cs        # Test Service Implementation
│   ├── Models/Mcp/              # MCP Protocol Models
│   │   ├── JsonRpcRequest.cs     # JSON-RPC Request Model
│   │   ├── JsonRpcResponse.cs    # JSON-RPC Response Model
│   │   ├── McpTool.cs           # MCP Tool Model
│   │   ├── McpContent.cs        # MCP Content Model
│   │   └── McpToolResult.cs     # MCP Tool Result Model
│   ├── Attributes/              # Tool Registration Attributes
│   │   └── McpToolAttribute.cs   # MCP Tool Attribute
│   ├── Base/                    # Base Classes
│   │   ├── BaseController.cs     # Base Controller
│   │   ├── BaseService.cs        # Base Service
│   │   └── IBaseService.cs       # Base Service Interface
│   ├── Extensions/              # Extension Methods
│   │   └── McpServiceExtensions.cs # MCP Service Extensions
│   ├── Common/                  # Common Classes
│   │   └── JsonHelper.cs         # JSON Helper Class
│   ├── Interfaces/              # Interface Definitions
│   │   └── ITestService.cs       # Test Service Interface
│   ├── Program.cs               # Program Entry Point
│   └── appsettings.json         # Configuration File
├── MCPSharp.Client/             # MCP Client
│   ├── ChatAIClient.cs          # AI Chat Client
│   └── Program.cs               # Client Entry Point
├── MCPSharp.sln                 # Solution File
├── LICENSE                      # License File
└── README.md                    # Project Documentation
```

## 🏗️ Software Architecture

MCPSharp is designed based on modern .NET architecture, supporting asynchronous communication, event-driven models, and modular extensions. Its core components include:

### Core Components

1. **Protocol Parser**: Efficiently parses MCP protocol data, supporting JSON-RPC 2.0 specification
2. **Network Service Layer**: High-performance network communication based on ASP.NET Core
3. **Event System**: Supports event subscription and publishing for business logic decoupling
4. **Plugin System**: Provides modular extension capabilities for easy functionality customization

### Architecture Features

- **Dependency Injection**: Uses .NET DI container to manage service lifecycles
- **Attribute-Driven**: Automatic tool registration based on `[McpTool]` attributes
- **Generic Design**: Type-safe base class design supporting strongly-typed parameters
- **Asynchronous Mode**: Fully asynchronous operation support for improved concurrent performance

## ✨ Core Features

### Server Features
- 🔧 **Automatic Tool Discovery**: Automatic tool registration using `[McpTool]` attributes
- 📡 **JSON-RPC 2.0**: Complete MCP protocol support
- 🎯 **Type Safety**: Strongly-typed parameter validation and serialization
- 🚀 **High Performance**: Asynchronous architecture based on ASP.NET Core
- 📝 **Swagger Integration**: Automatic API documentation generation

### Client Features
- 🤖 **AI Integration**: Support for OpenAI-compatible AI models
- 🔄 **Real-time Communication**: Real-time communication based on SSE (Server-Sent Events)
- 💬 **Streaming Response**: Support for streaming AI responses
- 🛠️ **Tool Invocation**: Automatic discovery and invocation of MCP tools

## 🛠️ Installation Guide

### Environment Requirements
- .NET 8.0 or higher
- Visual Studio 2022 or VS Code

### Clone and Build
```bash
# Clone the project
git clone https://github.com/xiaochanghai/MCPsharp
cd MCPSharp

# Restore packages and build
dotnet restore
dotnet build
```

## 🚀 Quick Start

### 1. Start the Server

```bash
cd MCPSharp.API
dotnet run
```

The server will start at `https://localhost:5196`, access Swagger documentation at: `https://localhost:5196/swagger`

### 2. Configure the Client

Edit the configuration in `MCPSharp.Client/ChatAIClient.cs`:

```csharp
private const string _apiKey = "your-api-key";
private const string _baseURL = "https://api.siliconflow.cn/v1";
private const string _modelID = "moonshotai/Kimi-K2-Instruct";
```

### 3. Start the Client

```bash
cd MCPSharp.Client
dotnet run
```

## 📖 Usage Instructions

### Creating MCP Tools

#### 1. Define Parameter Model
```csharp
public class InputSchemaArguments
{
    [Description("User name")]
    public string? name { get; set; }
}
```

#### 2. Implement Tool Method
```csharp
[McpTool("test_hello", "Greeting tool", typeof(InputSchemaArguments))]
public async Task<McpToolResult> HandleTestHello(object arguments)
{
    var args = JsonHelper.JsonToObj<InputSchemaArguments>(JsonHelper.ObjToJson(arguments));
    
    return new McpToolResult
    {
        Content = new[]
        {
            new McpContent
            {
                Type = "text",
                Text = $"Hello, {args.name}! Current time: {DateTime.Now}"
            }
        }
    };
}
```

#### 3. Register Service
```csharp
// In McpServiceExtensions.cs
services.AddScoped<ITestService, TestService>();
```

### MCP Protocol Endpoints

#### Initialize Connection
```http
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "initialize",
  "params": {
    "protocolVersion": "2024-11-05"
  },
  "id": 1
}
```

#### Get Tool List
```http
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "tools/list",
  "id": 2
}
```

#### Call Tool
```http
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "tools/call",
  "params": {
    "name": "test_hello",
    "arguments": {
      "name": "John"
    }
  },
  "id": 3
}
```

### Configuration

#### Server Configuration (appsettings.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### Client SSE Configuration
```csharp
var config = new SseClientTransport(
    new SseClientTransportOptions()
    {
        Endpoint = new Uri("http://localhost:5196/mcp"),
        UseStreamableHttp = true
    }
);
```

## 🧪 Test Examples

### Basic Test
After starting the server and client:

```
Question: Please say hello, my name is John
```

The AI will automatically call the `test_hello` tool and return greeting information.

### API Test
Use curl to test health check:
```bash
curl -X GET https://localhost:5196/
```

## 🚨 Troubleshooting

### Common Issues

1. **Port Occupation Error**
   - Modify the port in `launchSettings.json`
   - Or terminate the process occupying the port

2. **AI API Key Error**
   - Check the API key in `ChatAIClient.cs`
   - Confirm the API endpoint address is correct

3. **Tool Not Found**
   - Check `[McpTool]` attribute configuration
   - Confirm the service is registered in the DI container

### Debugging Tips
Enable verbose logging:
```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## 📚 API Reference

### McpToolAttribute
```csharp
[McpTool(name: "tool-name", description: "tool-description", inputSchemaType: typeof(ParameterType))]
```

### McpToolResult
```csharp
public class McpToolResult
{
    public McpContent[] Content { get; set; }
}
```

### McpContent
```csharp
public class McpContent
{
    public string Type { get; set; } = "text";
    public string Text { get; set; } = "";
}
```

## 🤝 Contributing

1. Fork this repository
2. Create a new branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push the branch: `git push origin feature/your-feature-name`
5. Create a Pull Request

### Code Standards
- Follow C# coding conventions
- Add necessary comments and documentation
- Write unit tests
- Keep code clean and readable

## 🎯 Roadmap

### Coming Soon
- [ ] Support for more AI model providers
- [ ] Add WebSocket transport support
- [ ] Implement Resources protocol
- [ ] Add Prompts functionality
- [ ] Support batch tool calls

### Long-term Plans
- [ ] Graphical management interface
- [ ] Performance monitoring and analysis
- [ ] Cluster deployment support
- [ ] Multi-language SDK support

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Related Links

- [Model Context Protocol Official Documentation](https://modelcontextprotocol.io/)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [OpenAI API Documentation](https://platform.openai.com/docs/)

## 📞 Support

For questions or suggestions, please create an [Issue](https://github.com/xiaochanghai/MCPsharp) or contact the maintainer.

---

⭐ If this project helps you, please give it a Star!