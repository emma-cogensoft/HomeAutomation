namespace HomeAutomation.WeatherForecastMetOffice.HttpAccessor;

public interface IWeatherForecastApiAccessor
{
    Task<string> GetAsJsonStringAsync(Uri uri, CancellationToken cancellationToken);
}