using Microsoft.Extensions.DependencyInjection;
using HomeAutomation.OctopusEnergy.ApiAccessor;
using HomeAutomation.OctopusEnergy.EnergyPricing;
using HomeAutomation.Application.Services.EnergyPricing;

namespace HomeAutomation.OctopusEnergy;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterOctopusEnergyServices(this IServiceCollection services)
    {
        services
            .AddHttpClient<IOctopusEnergyApiAccessor, OctopusEnergyApiAccessor>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "HomeAutomation/1.0");
            });

        services.AddScoped<IEnergyPricingReader, EnergyPricingReader>();

        return services;
    }
}
