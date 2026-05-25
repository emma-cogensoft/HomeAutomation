using HomeAutomation.Application.Services.EnergyPricing;

namespace HomeAutomation.OctopusEnergy.ApiAccessor;

public interface IOctopusEnergyApiAccessor
{
    Task<EnergyPricingData?> GetEnergyPricesAsync(CancellationToken cancellationToken = default);
}
