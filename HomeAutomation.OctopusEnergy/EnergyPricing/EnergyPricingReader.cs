using HomeAutomation.OctopusEnergy.ApiAccessor;
using HomeAutomation.Application.Services.EnergyPricing;

namespace HomeAutomation.OctopusEnergy.EnergyPricing;

public class EnergyPricingReader : IEnergyPricingReader
{
    private readonly IOctopusEnergyApiAccessor _apiAccessor;

    public EnergyPricingReader(IOctopusEnergyApiAccessor apiAccessor)
    {
        _apiAccessor = apiAccessor;
    }

    public async Task<EnergyPricingData?> GetPricingDataAsync(CancellationToken cancellationToken = default)
    {
        return await _apiAccessor.GetEnergyPricesAsync(cancellationToken);
    }
}
