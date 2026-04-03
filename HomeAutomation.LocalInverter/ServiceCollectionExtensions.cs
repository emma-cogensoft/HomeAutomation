using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.LocalInverter.ApiAccessor;
using HomeAutomation.LocalInverter.InverterSettings;
using HomeAutomation.LocalInverter.RealTimeData;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.LocalInverter;

public static class ServiceCollectionExtensions
{
    internal const string HttpClientName = "LocalInverter";

    public static void RegisterLocalInverterServices(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientName, c =>
        {
            c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            c.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<IInverterRealtimeDataReader, LocalInverterRealtimeDataReader>();
        services.AddScoped<IInverterSettingsDataReader, LocalInverterSettingsDataReader>();
        services.AddScoped<ILocalInverterApiAccessor, LocalInverterApiAccessor>();
    }
}