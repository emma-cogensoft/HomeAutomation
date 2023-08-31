using HomeAutomation.Application.Services.Weather;
using HomeAutomation.MetOffice.ApiAccessor;
using HomeAutomation.MetOffice.WeatherForecast;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.MetOffice;

public static class ServiceCollectionExtensions
{
    public static void RegisterWeatherServices(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastReader, WeatherForecastReader>();
        services.AddScoped<IWeatherForecastApiAccessor, WeatherForecastApiAccessor>();
    }
}