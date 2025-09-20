# MCPSharp

一个基于 .NET 8 实现的 Model Context Protocol (MCP) 服务端和客户端框架，支持 AI 助手与外部工具的交互。

## 🚀 项目简介

MCPSharp 是一个完整的 MCP 协议实现，旨在提供高性能、易扩展的网络通信能力，适用于各种 .NET 开发场景。项目包含：

- **MCPSharp.API**: MCP 服务端，提供工具注册和 JSON-RPC 接口
- **MCPSharp.Client**: MCP 客户端，支持与 AI 模型集成和工具调用

## 📁 项目结构

```markdown:README.md
MCPSharp/
├── MCPSharp.API/                 # MCP 服务端
│   ├── Controllers/              # API 控制器
│   │   └── TestController.cs     # 测试控制器
│   ├── Services/                 # 业务服务
│   │   └── TestService.cs        # 测试服务实现
│   ├── Models/Mcp/              # MCP 协议模型
│   │   ├── JsonRpcRequest.cs     # JSON-RPC 请求模型
│   │   ├── JsonRpcResponse.cs    # JSON-RPC 响应模型
│   │   ├── McpTool.cs           # MCP 工具模型
│   │   ├── McpContent.cs        # MCP 内容模型
│   │   └── McpToolResult.cs     # MCP 工具结果模型
│   ├── Attributes/              # 工具注册特性
│   │   └── McpToolAttribute.cs   # MCP 工具特性
│   ├── Base/                    # 基础类
│   │   ├── BaseController.cs     # 基础控制器
│   │   ├── BaseService.cs        # 基础服务
│   │   └── IBaseService.cs       # 基础服务接口
│   ├── Extensions/              # 扩展方法
│   │   └── McpServiceExtensions.cs # MCP 服务扩展
│   ├── Common/                  # 公共类
│   │   └── JsonHelper.cs         # JSON 助手类
│   ├── Interfaces/              # 接口定义
│   │   └── ITestService.cs       # 测试服务接口
│   ├── Program.cs               # 程序入口
│   └── appsettings.json         # 配置文件
├── MCPSharp.Client/             # MCP 客户端
│   ├── ChatAIClient.cs          # AI 聊天客户端
│   └── Program.cs               # 客户端入口
├── MCPSharp.sln                 # 解决方案文件
├── LICENSE                      # 许可证文件
└── README.md                    # 项目说明文档
```

## 🏗️ 软件架构

MCPSharp 基于现代 .NET 架构设计，支持异步通信、事件驱动模型以及模块化扩展。其核心组件包括：

### 核心组件

1. **协议解析器**：高效解析 MCP 协议数据，支持 JSON-RPC 2.0 规范
2. **网络服务层**：基于 ASP.NET Core 实现高性能网络通信
3. **事件系统**：支持事件订阅与发布，便于业务逻辑解耦
4. **插件系统**：提供模块化扩展能力，方便功能定制

### 架构特点

- **依赖注入**: 使用 .NET DI 容器管理服务生命周期
- **特性驱动**: 基于 `[McpTool]` 特性的工具自动注册
- **泛型设计**: 类型安全的基类设计，支持强类型参数
- **异步模式**: 全异步的操作支持，提高并发性能

## ✨ 核心特性

### 服务端特性
- 🔧 **工具自动发现**: 使用 `[McpTool]` 特性自动注册工具
- 📡 **JSON-RPC 2.0**: 完整的 MCP 协议支持
- 🎯 **类型安全**: 强类型的参数验证和序列化
- 🚀 **高性能**: 基于 ASP.NET Core 的异步架构
- 📝 **Swagger 集成**: 自动生成 API 文档

### 客户端特性
- 🤖 **AI 集成**: 支持 OpenAI 兼容的 AI 模型
- 🔄 **实时通信**: 基于 SSE (Server-Sent Events) 的实时通信
- 💬 **流式响应**: 支持流式 AI 响应
- 🛠️ **工具调用**: 自动发现和调用 MCP 工具

## 🛠️ 安装教程

### 环境要求
- .NET 8.0 或更高版本
- Visual Studio 2022 或 VS Code

### 克隆和构建
```bash
# 克隆项目
git clone https://github.com/xiaochanghai/MCPsharp
cd MCPSharp

# 还原包并构建
dotnet restore
dotnet build
```

## 🚀 快速开始

### 1. 启动服务端

```bash
cd MCPSharp.API
dotnet run
```

服务端将在 `https://localhost:5196` 启动，访问 Swagger 文档：`https://localhost:5196/swagger`

### 2. 配置客户端

编辑 `MCPSharp.Client/ChatAIClient.cs` 中的配置：

```csharp
private const string _apiKey = "your-api-key";
private const string _baseURL = "https://api.siliconflow.cn/v1";
private const string _modelID = "moonshotai/Kimi-K2-Instruct";
```

### 3. 启动客户端

```bash
cd MCPSharp.Client
dotnet run
```

## 📖 使用说明

### 创建 MCP 工具

#### 1. 定义参数模型
```csharp
public class InputSchemaArguments
{
    [Description("用户名称")]
    public string? name { get; set; }
}
```

#### 2. 实现工具方法
```csharp
[McpTool("test_hello", "问候工具", typeof(InputSchemaArguments))]
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
                Text = $"Hello, {args.name}! 当前时间: {DateTime.Now}"
            }
        }
    };
}
```

#### 3. 注册服务
```csharp
// 在 McpServiceExtensions.cs 中
services.AddScoped<ITestService, TestService>();
```

### MCP 协议端点

#### 初始化连接
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

#### 获取工具列表
```http
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "tools/list",
  "id": 2
}
```

#### 调用工具
```http
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "tools/call",
  "params": {
    "name": "test_hello",
    "arguments": {
      "name": "张三"
    }
  },
  "id": 3
}
```

### 配置说明

#### 服务端配置 (appsettings.json)
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

#### 客户端 SSE 配置
```csharp
var config = new SseClientTransport(
    new SseClientTransportOptions()
    {
        Endpoint = new Uri("http://localhost:5196/mcp"),
        UseStreamableHttp = true
    }
);
```

## 🧪 测试示例

### 基本测试
启动服务端和客户端后：

```
提问: 请说声你好，我的名字是李四
```

AI 将自动调用 `test_hello` 工具并返回问候信息。

### API 测试
使用 curl 测试健康检查：
```bash
curl -X GET https://localhost:5196/
```

## 🚨 故障排除

### 常见问题

1. **端口占用错误**
   - 修改 `launchSettings.json` 中的端口
   - 或终止占用端口的进程

2. **AI API 密钥错误**
   - 检查 `ChatAIClient.cs` 中的 API 密钥
   - 确认 API 端点地址正确

3. **工具未发现**
   - 检查 `[McpTool]` 特性配置
   - 确认服务已在 DI 容器中注册

### 调试技巧
启用详细日志：
```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## 📚 API 参考

### McpToolAttribute
```csharp
[McpTool(name: "工具名称", description: "工具描述", inputSchemaType: typeof(参数类型))]
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

## 🤝 参与贡献

1. Fork 本仓库
2. 创建新分支：`git checkout -b feature/your-feature-name`
3. 提交更改：`git commit -am 'Add some feature'`
4. 推送分支：`git push origin feature/your-feature-name`
5. 创建 Pull Request

### 代码规范
- 遵循 C# 编码规范
- 添加必要的注释和文档
- 编写单元测试
- 保持代码整洁和可读性

## 🎯 路线图

### 即将推出
- [ ] 支持更多 AI 模型提供商
- [ ] 添加 WebSocket 传输支持
- [ ] 实现资源（Resources）协议
- [ ] 添加提示（Prompts）功能
- [ ] 支持批量工具调用

### 长期计划
- [ ] 图形化管理界面
- [ ] 性能监控和分析
- [ ] 集群部署支持
- [ ] 多语言 SDK 支持

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🔗 相关链接

- [Model Context Protocol 官方文档](https://modelcontextprotocol.io/)
- [.NET 8 文档](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core 文档](https://docs.microsoft.com/en-us/aspnet/core/)
- [OpenAI API 文档](https://platform.openai.com/docs/)

## 📞 支持

如有问题或建议，请创建 [Issue](https://github.com/xiaochanghai/MCPsharp) 或联系维护者。

---

⭐ 如果这个项目对您有帮助，请给个 Star！