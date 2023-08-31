using Microsoft.AspNetCore.Mvc;

namespace HomeAutomation.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public async Task<int> GetAsync()
    {
        await Task.CompletedTask;
        return 1;
    }
}