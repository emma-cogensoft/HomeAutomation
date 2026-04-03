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
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var inverterSettings = await _mediator.Send(new GetInverterSettings());

            _logger.LogDebug("Inverter settings retrieved: {CurrentSetting}", inverterSettings.CurrentWorkTypeName);

            return Ok(new InverterSettingsResponse(inverterSettings.TimeStamp, inverterSettings.CurrentWorkTypeName, true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve inverter settings");
            return StatusCode(503, "Inverter settings are temporarily unavailable.");
        }
    }

    public record InverterSettingsResponse(DateTime TimeStamp, string CurrentSettingName, bool IsLoaded);
}
