using HomeAutomation.Application.Services.EnergyPricing;
using HomeAutomation.OctopusEnergy;
using HomeAutomation.OctopusEnergy.ApiAccessor;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HomeAutomation.Application.Tests;

public class OctopusEnergyApiAccessorTests
{
    [Fact]
    public async Task GetEnergyPricesAsync_WithValidResponse_ReturnsPricingData()
    {
        // Arrange
        var httpClient = new HttpClient(new MockHttpMessageHandler(validOctopusResponse))
        {
            BaseAddress = new Uri("https://api.octopus.energy")
        };

        var options = Options.Create(new OctopusEnergyApiOptions
        {
            Tariff = "COSY-FIX-12M-26-03-23",
            RegionCode = "H",
            HoursAhead = 24
        });

        var accessor = new OctopusEnergyApiAccessor(httpClient, options);

        // Act
        var result = await accessor.GetEnergyPricesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Prices);
        Assert.NotEmpty(result.Prices);
        Assert.NotNull(result.CurrentPrice);
    }

    [Fact]
    public async Task GetEnergyPricesAsync_ParsesNullValidToAsMaxValue()
    {
        // Arrange
        var httpClient = new HttpClient(new MockHttpMessageHandler(validOctopusResponse))
        {
            BaseAddress = new Uri("https://api.octopus.energy")
        };

        var options = Options.Create(new OctopusEnergyApiOptions
        {
            Tariff = "COSY-FIX-12M-26-03-23",
            RegionCode = "H",
            HoursAhead = 24
        });

        var accessor = new OctopusEnergyApiAccessor(httpClient, options);

        // Act
        var result = await accessor.GetEnergyPricesAsync();

        // Assert
        Assert.NotNull(result);
        var priceWithMaxValue = result.Prices.FirstOrDefault(p => p.ValidTo == DateTime.MaxValue);
        Assert.NotNull(priceWithMaxValue);
    }

    [Fact]
    public async Task GetEnergyPricesAsync_IdentifiesCurrentPrice()
    {
        // Arrange
        var httpClient = new HttpClient(new MockHttpMessageHandler(validOctopusResponse))
        {
            BaseAddress = new Uri("https://api.octopus.energy")
        };

        var options = Options.Create(new OctopusEnergyApiOptions
        {
            Tariff = "COSY-FIX-12M-26-03-23",
            RegionCode = "H",
            HoursAhead = 24
        });

        var accessor = new OctopusEnergyApiAccessor(httpClient, options);

        // Act
        var result = await accessor.GetEnergyPricesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CurrentPrice);
        Assert.True(result.CurrentPrice.IsActive);
    }

    [Fact]
    public async Task GetEnergyPricesAsync_ConstructsTariffCodeCorrectly()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(validOctopusResponse);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.octopus.energy")
        };

        var options = Options.Create(new OctopusEnergyApiOptions
        {
            Tariff = "COSY-FIX-12M-26-03-23",
            RegionCode = "H",
            HoursAhead = 24
        });

        var accessor = new OctopusEnergyApiAccessor(httpClient, options);

        // Act
        await accessor.GetEnergyPricesAsync();

        // Assert
        var requestUri = mockHandler.LastRequestUri;
        Assert.NotNull(requestUri);
        Assert.Contains("E-1R-COSY-FIX-12M-26-03-23-H", requestUri.ToString());
    }

    private static readonly string validOctopusResponse = """
        {
            "count": 1,
            "next": null,
            "previous": null,
            "results": [
                {
                    "value_inc_vat": 14.497140,
                    "value_exc_vat": 12.084300,
                    "valid_from": "2026-05-25T12:00:00Z",
                    "valid_to": "2026-05-25T15:00:00Z"
                },
                {
                    "value_inc_vat": 15.497140,
                    "value_exc_vat": 13.084300,
                    "valid_from": "2026-05-25T15:00:00Z",
                    "valid_to": null
                }
            ]
        }
        """;
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;
    public Uri? LastRequestUri { get; private set; }

    public MockHttpMessageHandler(string response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(_response)
        };
        response.Content.Headers.ContentType = new("application/json");
        return Task.FromResult(response);
    }
}
