using MCPSharp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MCPSharp.API.Controllers;

/// <summary>
/// Test
/// </summary>
[Route("/")]
public class TestController : BaseController<ITestService>
{
    public TestController(ITestService service, ILogger<TestController> logger) : base(service, logger)
    {
    }
}