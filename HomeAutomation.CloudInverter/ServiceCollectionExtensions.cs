using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.CloudInverter.ApiAccessor;
using HomeAutomation.CloudInverter.RealTimeData;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.CloudInverter;

public static class ServiceCollectionExtensions
{
    public static void RegisterCloudInverterServices(this IServiceCollection services)
    {
        services.AddScoped<IInverterRealtimeDataReader, CloudInverterRealtimeDataReader>();
        services.AddScoped<ICloudInverterApiAccessor, CloudInverterApiAccessor>();
    }
}