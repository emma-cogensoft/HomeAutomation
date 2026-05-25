using HomeAutomation.Application.Services.EnergyPricing;

namespace HomeAutomation.OctopusEnergy.ApiAccessor;

public record OctopusEnergyPriceResponse(
    string Url,
    Result Results
);

public record Result(
    int Count,
    string? Next,
    string? Previous,
    List<PricePoint> Results
);

public record PricePoint(
    decimal VatInclusivePrice,
    decimal VatExclusivePrice,
    DateTime ValidFrom,
    DateTime ValidTo
);
