using HomeAutomation.Application.WeatherForecast;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeAutomation.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForecastController : ControllerBase
{
    private readonly ILogger<ForecastController> _logger;
    private readonly IMediator _mediator;

    public ForecastController(ILogger<ForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var weather = await _mediator.Send(new GetWeatherForecast());

            _logger.LogDebug("Weather forecast retrieved: {WeatherType}, IsSunny: {IsSunny}",
                weather.WeatherType, weather.IsSunny);

            return Ok(new WeatherResponse(weather.IsSunny));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve weather forecast");
            return StatusCode(503, "Weather forecast is temporarily unavailable.");
        }
    }

    public record WeatherResponse(bool IsSunny, bool IsLoading = false);
}
