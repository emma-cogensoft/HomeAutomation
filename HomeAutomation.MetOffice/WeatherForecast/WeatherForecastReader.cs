using HomeAutomation.Application.Services.Weather;
using HomeAutomation.Domain.Weather;
using HomeAutomation.MetOffice.ApiAccessor;
using Microsoft.Extensions.Options;

namespace HomeAutomation.MetOffice.WeatherForecast;

public class WeatherForecastReader : IWeatherForecastReader
{
    private readonly IWeatherForecastApiAccessor _httpAccessor;
    private readonly OpenMeteoApiOptions _options;

    public WeatherForecastReader(IWeatherForecastApiAccessor httpAccessor, IOptions<OpenMeteoApiOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(httpAccessor);

        _httpAccessor = httpAccessor;
        _options = options.Value;
    }

    public async Task<Domain.Weather.WeatherForecast> GetForecastForToday(CancellationToken cancellationToken)
    {
        var uri = BuildUri(_options.Latitude, _options.Longitude);
        var response = await _httpAccessor.GetAsync<OpenMeteoResponse>(uri, cancellationToken);

        var isDay = response.CurrentWeather.IsDay == 1;
        var weatherType = MapWmoCode(response.CurrentWeather.WeatherCode, isDay);

        return new Domain.Weather.WeatherForecast(weatherType);
    }

    private static Uri BuildUri(double latitude, double longitude) =>
        new($"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true");

    private static WeatherType MapWmoCode(int wmoCode, bool isDay) => wmoCode switch
    {
        0 or 1 => isDay ? WeatherType.SunnyDay : WeatherType.ClearNight,
        2      => isDay ? WeatherType.PartlyCloudyDay : WeatherType.PartlyCloudyNight,
        3      => WeatherType.Overcast,
        45 or 48 => WeatherType.Fog,
        51 or 53 or 55 => WeatherType.Drizzle,
        61 or 63 => WeatherType.LightRain,
        65     => WeatherType.HeavyRain,
        66 or 67 => WeatherType.Sleet,
        71 or 73 => WeatherType.LightSnow,
        75     => WeatherType.HeavySnow,
        77     => WeatherType.Sleet,
        80 or 81 => isDay ? WeatherType.LightRainShowerDay : WeatherType.LightRainShowerNight,
        82     => isDay ? WeatherType.HeavyRainShowerDay : WeatherType.HeavyRainShowerNight,
        85     => isDay ? WeatherType.LightSnowShowerDay : WeatherType.LightSnowShowerNight,
        86     => isDay ? WeatherType.HeavySnowShowerDay : WeatherType.HeavySnowShowerNight,
        95     => WeatherType.Thunder,
        96 or 99 => isDay ? WeatherType.HailShowerDay : WeatherType.HailShowerNight,
        _      => WeatherType.Overcast
    };
}