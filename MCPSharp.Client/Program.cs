using MCPSharp.Client;
using ModelContextProtocol.Client; // 包含 McpClientFactory 和 McpClient 相关定义
using ModelContextProtocol.Protocol.Transport; // 包含传输层相关类，如 SseClientTransport

// 创建一个 SSE（Server-Sent Events）客户端传输配置实例
var config = new SseClientTransport(
    new SseClientTransportOptions()
    {
        // 设置远程服务器的 URI 地址 (记得替换真实的地址，从魔搭MCP广场获取)
        Endpoint = new Uri("http://localhost:8016/Supplier/mcp"),
        UseStreamableHttp = true
    }
);

// 使用配置创建 MCP 客户端实例
var client = await McpClientFactory.CreateAsync(config);

// 调用客户端的 ListToolsAsync 方法，获取可用工具列表
var listToolsResult = await client.ListToolsAsync();

Console.WriteLine("功能列表:");
// 遍历工具列表，并逐个输出到控制台
foreach (var tool in listToolsResult)
{
    Console.WriteLine($"  名称：{tool.Name}，说明：{tool.Description}");
}

// 输出欢迎信息，提示用户开始与 MCP AI 交互
Console.WriteLine("MCP Client成功启动，开启体验！输入 exit 退出！");

// 创建聊天客户端实例
ChatAIClient chatAIClient = new ChatAIClient();

// 进入主循环，持续接收用户输入直到输入 "exit"
while (true)
{
    try
    {
        // 设置控制台文字颜色为黄色，提示用户输入问题
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\n提问: ");

        // 读取用户输入并去除前后空格，若为空则赋默认空字符串
        string query = Console.ReadLine()?.Trim() ?? string.Empty;

        // 判断用户是否输入 "exit" 以退出程序
        if (query.ToLower() == "exit")
        {
            break;
        }

        // 调用异步方法处理用户查询，并传入预定义的工具列表（listToolsResult）
        string response = await chatAIClient.ProcessQueryAsync(query, listToolsResult); 
        // 设置输出颜色为黄色，显示 AI 的响应内容
        Console.ForegroundColor = ConsoleColor.Yellow;
        //Console.WriteLine($"AI：{response}");

        // 恢复控制台默认颜色（白色）
        Console.ForegroundColor = ConsoleColor.White;
    }
    catch (Exception ex)
    {
        // 捕获所有异常并输出错误信息，防止程序崩溃
        Console.WriteLine($"\nError: {ex.Message}");
    }
}