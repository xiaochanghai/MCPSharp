using MCPSharp.Client;
using ModelContextProtocol.Client; // Contains definitions for McpClientFactory and McpClient

// Create an SSE (Server-Sent Events) client transport configuration instance
var config = new HttpClientTransport(
    new HttpClientTransportOptions()
    {
        // Set the URI address of the remote server (remember to replace with the actual address from ModelScope MCP Plaza)
        Endpoint = new Uri("http://localhost:5196/mcp"),
    }
);

// Create an MCP client instance using the above configuration
    var client = await McpClient.CreateAsync(config);

// Call the client's ListToolsAsync method to retrieve the list of available tools
var listToolsResult = await client.ListToolsAsync();

Console.WriteLine("Available Tools:");
// Iterate through the tool list and print each one to the console
foreach (var tool in listToolsResult)
{
    Console.WriteLine($"  Name: {tool.Name}, Description: {tool.Description}");
}

// Print welcome message, prompting user to start interacting with MCP AI
Console.WriteLine("MCP Client started successfully. Start your experience! Type 'exit' to quit.");

// Create an instance of the chat client
ChatAIClient chatAIClient = new ChatAIClient();

// Enter main loop: continuously accept user input until "exit" is entered
while (true)
{
    try
    {
        // Set console text color to yellow to prompt user for input
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nAsk: ");

        // Read user input, trim whitespace; assign empty string if null
        string query = Console.ReadLine()?.Trim() ?? string.Empty;

        // Check if user typed "exit" to quit the program
        if (query.ToLower() == "exit")
        {
            break;
        }

        // Call async method to process user query, passing in the pre-fetched tool list (listToolsResult)
        string response = await chatAIClient.ProcessQueryAsync(query, listToolsResult);
        // Set output color to yellow to display AI's response
        Console.ForegroundColor = ConsoleColor.Yellow;
        //Console.WriteLine($"AI: {response}");

        // Reset console color to default (white)
        Console.ForegroundColor = ConsoleColor.White;
    }
    catch (Exception ex)
    {
        // Catch any exception and print error message to prevent crash
        Console.WriteLine($"\nError: {ex.Message}");
    }
}