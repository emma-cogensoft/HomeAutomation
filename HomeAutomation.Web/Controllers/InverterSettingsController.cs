using HomeAutomation.Application.InverterSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeAutomation.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InverterSettingsController : ControllerBase
{
    private readonly ILogger<InverterSettingsController> _logger;
    private readonly IMediator _mediator;

    public InverterSettingsController(ILogger<InverterSettingsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<InverterSettingsResponse> GetAsync()
    {
        var inverterSettings = await _mediator.Send(new GetInverterSettings());
        
        return new InverterSettingsResponse(inverterSettings.TimeStamp, inverterSettings.CurrentWorkTypeName, true);
    }

    public record InverterSettingsResponse(DateTime TimeStamp, string CurrentSettingName, bool IsLoaded);
}
