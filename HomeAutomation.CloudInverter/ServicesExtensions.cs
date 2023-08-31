using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.CloudInverter.InverterApiRealTimeData;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.CloudInverter;

public static class ServicesExtensions
{
    public static void RegisterCloudInverterServices(this IServiceCollection services)
    {
        services.AddScoped<IInverterRealtimeDataReader, CloudInverterRealtimeDataReader>();
        services.AddScoped<ICloudInverterApiAccessor, CloudInverterApiAccessor>();
    }
}