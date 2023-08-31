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
    public async Task<WeatherResponse> GetAsync()
    {
        var weather = await _mediator.Send(new GetWeatherForecast());
        
        return new WeatherResponse(weather.IsSunny);
    }

    public record WeatherResponse(bool IsSunny, bool IsLoading = false);
}
