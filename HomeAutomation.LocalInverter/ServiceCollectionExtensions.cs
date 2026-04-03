using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.LocalInverter.ApiAccessor;
using HomeAutomation.LocalInverter.InverterSettings;
using HomeAutomation.LocalInverter.RealTimeData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

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
        })
        .AddStandardResilienceHandler(options =>
        {
            // Local network: fewer retries with shorter delays
            options.Retry.MaxRetryAttempts = 2;
            options.Retry.Delay = TimeSpan.FromMilliseconds(500);
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IInverterRealtimeDataReader, LocalInverterRealtimeDataReader>();
        services.AddScoped<IInverterSettingsDataReader, LocalInverterSettingsDataReader>();
        services.AddScoped<ILocalInverterApiAccessor, LocalInverterApiAccessor>();
    }
}