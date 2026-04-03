using HomeAutomation.Application.BatteryData;
using HomeAutomation.Application.Services.Inverter;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.Application;

public static class ServiceCollectionExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        services.AddScoped<IInverterRealtimeDataReader, FallbackInverterRealtimeDataReader>();
    }
}