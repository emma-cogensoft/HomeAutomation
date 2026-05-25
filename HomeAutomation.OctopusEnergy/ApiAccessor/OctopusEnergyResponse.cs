using HomeAutomation.Application.Services.EnergyPricing;

namespace HomeAutomation.OctopusEnergy.ApiAccessor;

public record OctopusEnergyPriceResponse(
    int Count,
    string? Next,
    string? Previous,
    List<PricePoint> Results
);

public record PricePoint(
    [property: System.Text.Json.Serialization.JsonPropertyName("value_inc_vat")]
    decimal VatInclusivePrice,
    [property: System.Text.Json.Serialization.JsonPropertyName("value_exc_vat")]
    decimal VatExclusivePrice,
    [property: System.Text.Json.Serialization.JsonPropertyName("valid_from")]
    DateTime ValidFrom,
    [property: System.Text.Json.Serialization.JsonPropertyName("valid_to")]
    DateTime? ValidTo
);
