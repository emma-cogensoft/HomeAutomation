using HomeAutomation.Application.Services.Weather;
using HomeAutomation.MetOffice.ApiAccessor;
using HomeAutomation.MetOffice.WeatherForecast;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.MetOffice;

public static class ServiceCollectionExtensions
{
    internal const string HttpClientName = "MetOffice";

    public static void RegisterWeatherServices(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientName, c =>
        {
            c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            c.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IWeatherForecastReader, WeatherForecastReader>();
        services.AddScoped<IWeatherForecastApiAccessor, WeatherForecastApiAccessor>();
    }
}