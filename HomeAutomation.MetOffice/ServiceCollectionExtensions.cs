using HomeAutomation.Application.Services.Weather;
using HomeAutomation.MetOffice.HttpAccessor;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.MetOffice;

public static class ServiceCollectionExtensions
{
    public static void RegisterWeatherServices(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastReader, WeatherForecaster>();
        services.AddScoped<IWeatherForecastApiAccessor, WeatherForecastApiAccessor>();
    }
}