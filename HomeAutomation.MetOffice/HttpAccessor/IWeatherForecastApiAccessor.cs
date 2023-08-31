namespace HomeAutomation.MetOffice.HttpAccessor;

public interface IWeatherForecastApiAccessor
{
    Task<string> GetAsJsonStringAsync(Uri uri, CancellationToken cancellationToken);
}