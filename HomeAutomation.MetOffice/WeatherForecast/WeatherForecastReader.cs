using HomeAutomation.Application.Services.Weather;
using HomeAutomation.Domain.Weather;
using HomeAutomation.MetOffice.HttpAccessor;
using Microsoft.Extensions.Options;

namespace HomeAutomation.MetOffice.WeatherForecast;

public class WeatherForecastReader: IWeatherForecastReader
{
    private readonly IWeatherForecastApiAccessor _httpAccessor;
    private readonly MetOfficeApiSettingsOptions _options;

    public WeatherForecastReader(IWeatherForecastApiAccessor httpAccessor, IOptions<MetOfficeApiSettingsOptions> options)
    {
        _httpAccessor = httpAccessor;
        _options = options.Value;
    }
    
    public async Task<Domain.Weather.WeatherForecast> GetForecastForToday(CancellationToken cancellationToken)
    {
        var uri = new Uri(_options.RequestUri);
        
        var response = await _httpAccessor.GetAsync<RootObject>(uri, cancellationToken);
        
        if (response == null) throw new MetOfficeApiException("Could not get forecast");

        var report = response.SiteRep.DV.Location.Period[0].Rep[0];
        
        var metOfficeWeatherType = (MetOfficeWeatherType)Enum.Parse(typeof(MetOfficeWeatherType), report.W);
        var weatherType = MapWeatherType(metOfficeWeatherType);
        
        return new Domain.Weather.WeatherForecast(weatherType);
    }

    private static WeatherType MapWeatherType(MetOfficeWeatherType metOfficeWeatherType)
    {
        // for now, just an exact one to one mapping
        return (WeatherType)Enum.Parse(typeof(WeatherType), metOfficeWeatherType.ToString());
    }
}