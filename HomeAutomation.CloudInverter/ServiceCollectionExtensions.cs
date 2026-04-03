using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.CloudInverter.ApiAccessor;
using HomeAutomation.CloudInverter.RealTimeData;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.CloudInverter;

public static class ServiceCollectionExtensions
{
    internal const string HttpClientName = "CloudInverter";

    public static void RegisterCloudInverterServices(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientName, c =>
        {
            c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            c.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IInverterRealtimeDataReader, CloudInverterRealtimeDataReader>();
        services.AddScoped<ICloudInverterApiAccessor, CloudInverterApiAccessor>();
    }
}