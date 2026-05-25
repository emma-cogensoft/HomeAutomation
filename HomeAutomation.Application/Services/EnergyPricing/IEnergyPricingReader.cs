namespace HomeAutomation.Application.Services.EnergyPricing;

public record EnergyPrice(
    DateTime ValidFrom,
    DateTime ValidTo,
    decimal UnitRateInclVat,
    decimal UnitRateExclVat
)
{
    public bool IsActive => DateTime.UtcNow >= ValidFrom && DateTime.UtcNow < ValidTo;
}

public record EnergyPricingData(
    List<EnergyPrice> Prices,
    EnergyPrice? CurrentPrice,
    EnergyPrice? NextCheapestPeriod,
    decimal? CurrentUnitRate,
    decimal? AverageRate
);

public interface IEnergyPricingReader
{
    Task<EnergyPricingData?> GetPricingDataAsync(CancellationToken cancellationToken = default);
}
