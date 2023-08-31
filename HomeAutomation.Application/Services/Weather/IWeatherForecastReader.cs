namespace HomeAutomation.Application.Services.Weather;

public interface IWeatherForecastReader
{
    public Task<Domain.Weather.WeatherForecast> GetForecastForToday(CancellationToken cancellationToken);
}