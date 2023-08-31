namespace HomeAutomation.MetOffice.HttpAccessor;

public interface IWeatherForecastApiAccessor
{
    Task<T> GetAsync<T>(Uri uri, CancellationToken cancellationToken);
}