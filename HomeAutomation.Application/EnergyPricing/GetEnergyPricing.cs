using MediatR;
using HomeAutomation.Application.Services.EnergyPricing;

namespace HomeAutomation.Application.EnergyPricing;

public record GetEnergyPricing : IRequest<EnergyPricingResponse?>;

public record EnergyPricingResponse(
    decimal? CurrentUnitRate,
    decimal? CurrentUnitRatePence,
    EnergyPriceDetail? CurrentPeriod,
    EnergyPriceDetail? NextCheapestPeriod,
    decimal? AverageRate,
    decimal? AverageRatePence,
    List<EnergyPriceDetail> Prices24H
);

public record EnergyPriceDetail(
    DateTime ValidFrom,
    DateTime ValidTo,
    decimal UnitRateInclVat,
    decimal UnitRateInclVatPence,
    bool IsActive,
    bool IsCheapest
);

public class GetEnergyPricingHandler : IRequestHandler<GetEnergyPricing, EnergyPricingResponse?>
{
    private readonly Application.Services.EnergyPricing.IEnergyPricingReader _reader;

    public GetEnergyPricingHandler(Application.Services.EnergyPricing.IEnergyPricingReader reader)
    {
        _reader = reader;
    }

    public async Task<EnergyPricingResponse?> Handle(GetEnergyPricing request, CancellationToken cancellationToken)
    {
        var pricingData = await _reader.GetPricingDataAsync(cancellationToken);

        if (pricingData == null)
            return null;

        var minPrice = pricingData.Prices.Count > 0 ? pricingData.Prices.Min(p => p.UnitRateInclVat) : 0m;

        var prices24h = pricingData.Prices
            .Select(p => new EnergyPriceDetail(
                p.ValidFrom,
                p.ValidTo,
                p.UnitRateInclVat,
                p.UnitRateInclVat * 100,
                p.IsActive,
                p.UnitRateInclVat == minPrice))
            .ToList();

        return new EnergyPricingResponse(
            pricingData.CurrentUnitRate,
            pricingData.CurrentUnitRate * 100,
            pricingData.CurrentPrice != null
                ? new EnergyPriceDetail(
                    pricingData.CurrentPrice.ValidFrom,
                    pricingData.CurrentPrice.ValidTo,
                    pricingData.CurrentPrice.UnitRateInclVat,
                    pricingData.CurrentPrice.UnitRateInclVat * 100,
                    true,
                    pricingData.CurrentPrice.UnitRateInclVat == minPrice)
                : null,
            pricingData.NextCheapestPeriod != null
                ? new EnergyPriceDetail(
                    pricingData.NextCheapestPeriod.ValidFrom,
                    pricingData.NextCheapestPeriod.ValidTo,
                    pricingData.NextCheapestPeriod.UnitRateInclVat,
                    pricingData.NextCheapestPeriod.UnitRateInclVat * 100,
                    pricingData.NextCheapestPeriod.IsActive,
                    true)
                : null,
            pricingData.AverageRate,
            pricingData.AverageRate * 100,
            prices24h
        );
    }
}
