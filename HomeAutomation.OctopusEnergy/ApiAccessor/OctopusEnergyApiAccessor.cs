using Microsoft.Extensions.Options;
using HomeAutomation.Application.Services.EnergyPricing;

namespace HomeAutomation.OctopusEnergy.ApiAccessor;

public class OctopusEnergyApiAccessor : IOctopusEnergyApiAccessor
{
    private readonly HttpClient _httpClient;
    private readonly OctopusEnergyApiOptions _options;

    public OctopusEnergyApiAccessor(HttpClient httpClient, IOptions<OctopusEnergyApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<EnergyPricingData?> GetEnergyPricesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var periodFrom = now.AddHours(-1);
            var periodTo = now.AddHours(_options.HoursAhead);

            var productCode = _options.Tariff.Split('-')[0];
            var url =
                $"https://api.octopusenergy.com/v1/products/{productCode}/electricity-tariffs/{_options.Tariff}{_options.RegionCode}/standard-unit-rates/" +
                $"?period_from={periodFrom:O}&period_to={periodTo:O}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new OctopusEnergyException(
                    $"Octopus Energy API returned {response.StatusCode}: {await response.Content.ReadAsStringAsync(cancellationToken)}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var octopusResponse = System.Text.Json.JsonSerializer.Deserialize<OctopusEnergyPriceResponse>(
                jsonContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (octopusResponse?.Results?.Results == null || octopusResponse.Results.Results.Count == 0)
            {
                throw new OctopusEnergyException("No price data returned from Octopus Energy API");
            }

            var prices = octopusResponse.Results.Results
                .Select(p => new EnergyPrice(
                    p.ValidFrom,
                    p.ValidTo,
                    p.VatInclusivePrice,
                    p.VatExclusivePrice))
                .OrderBy(p => p.ValidFrom)
                .ToList();

            var currentPrice = prices.FirstOrDefault(p => p.IsActive);
            var currentUnitRate = currentPrice?.UnitRateInclVat;

            var futurePrices = prices.Where(p => p.ValidFrom > now).ToList();
            var cheapestFuture = futurePrices.OrderBy(p => p.UnitRateInclVat).FirstOrDefault();

            var averageRate = prices.Count > 0
                ? (decimal?)prices.Average(p => (double)p.UnitRateInclVat)
                : null;

            return new EnergyPricingData(
                prices,
                currentPrice,
                cheapestFuture,
                currentUnitRate,
                averageRate);
        }
        catch (HttpRequestException ex)
        {
            throw new OctopusEnergyException("Failed to connect to Octopus Energy API", ex);
        }
    }
}
