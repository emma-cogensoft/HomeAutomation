using System.Text.Json;
using HomeAutomation.Application.Services.Weather;
using HomeAutomation.Domain.Weather;
using HomeAutomation.WeatherForecastMetOffice.HttpAccessor;
using Microsoft.Extensions.Options;

namespace HomeAutomation.WeatherForecastMetOffice;

public class WeatherForecaster: IWeatherForecastReader
{
    private readonly IWeatherForecastApiAccessor _httpAccessor;
    private readonly MetOfficeApiSettingsOptions _options;

    public WeatherForecaster(IWeatherForecastApiAccessor httpAccessor, IOptions<MetOfficeApiSettingsOptions> options)
    {
        _httpAccessor = httpAccessor;
        _options = options.Value;
    }
    
    public async Task<WeatherForecast> GetForecastForToday(CancellationToken cancellationToken)
    {
        var uri = new Uri(_options.RequestUri);
        
        var response = await _httpAccessor.GetAsJsonStringAsync(uri, cancellationToken);
        
        var result = JsonSerializer.Deserialize<RootObject>(response);
        
        if (result == null) throw new WeatherForecastMetOfficeApiException("Could not get forecast");

        var report = result.SiteRep.DV.Location.Period[0].Rep[0];
        
        var weatherType = (WeatherType)Enum.Parse(typeof(WeatherType), report.W);
        
        return new WeatherForecast(weatherType);
    }
}