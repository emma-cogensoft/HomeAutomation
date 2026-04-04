using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.CloudInverter.ApiAccessor;
using HomeAutomation.CloudInverter.RealTimeData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

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
        })
        .AddStandardResilienceHandler(options =>
        {
            // Exponential backoff with jitter handles 429 (TooManyRequests) and transient errors.
            // MaxDelay caps the wait so a page load doesn't hang indefinitely.
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.BackoffType = DelayBackoffType.Exponential;
            options.Retry.UseJitter = true;
            options.Retry.Delay = TimeSpan.FromSeconds(2);
            options.Retry.MaxDelay = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
        });

        services.AddKeyedScoped<IInverterRealtimeDataReader, CloudInverterRealtimeDataReader>("cloud");
        services.AddScoped<ICloudInverterApiAccessor, CloudInverterApiAccessor>();
    }
}