namespace HomeAutomation.MetOffice.ApiAccessor;

public interface IWeatherForecastApiAccessor
{
    Task<T> GetAsync<T>(Uri uri, CancellationToken cancellationToken);
}