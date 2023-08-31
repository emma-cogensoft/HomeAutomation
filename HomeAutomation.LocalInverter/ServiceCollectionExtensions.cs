using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.LocalInverter.ApiAccessor;
using HomeAutomation.LocalInverter.InverterSettings;
using HomeAutomation.LocalInverter.RealTimeData;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.LocalInverter;

public static class ServiceCollectionExtensions
{
    public static void RegisterLocalInverterServices(this IServiceCollection services)
    {
        services.AddScoped<IInverterRealtimeDataReader, LocalInverterRealtimeDataReader>();
        services.AddScoped<IInverterSettingsDataReader, LocalInverterSettingsDataReader>();
        services.AddScoped<ILocalInverterApiAccessor, LocalInverterApiAccessor>();
    }
}