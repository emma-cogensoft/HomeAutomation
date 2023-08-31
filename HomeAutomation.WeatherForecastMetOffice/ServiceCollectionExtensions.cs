using HomeAutomation.Application.Services.Weather;
using HomeAutomation.WeatherForecastMetOffice.HttpAccessor;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.WeatherForecastMetOffice;

public static class ServiceCollectionExtensions
{
    public static void RegisterWeatherServices(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastReader, WeatherForecaster>();
        services.AddScoped<IWeatherForecastApiAccessor, WeatherForecastApiAccessor>();
    }
}