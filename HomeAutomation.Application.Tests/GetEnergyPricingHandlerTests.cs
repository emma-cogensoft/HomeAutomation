using HomeAutomation.Application.EnergyPricing;
using HomeAutomation.Application.Services.EnergyPricing;
using Moq;
using Xunit;

namespace HomeAutomation.Application.Tests;

public class GetEnergyPricingHandlerTests
{
    [Fact]
    public async Task Handle_WithValidPricingData_ReturnsFormattedResponse()
    {
        // Arrange
        var pricingData = new EnergyPricingData(
            Prices: new List<EnergyPrice>
            {
                new(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, 14.50m, 12.08m),
                new(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 15.00m, 12.50m),
            },
            CurrentPrice: new EnergyPrice(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, 14.50m, 12.08m),
            NextCheapestPeriod: new EnergyPrice(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 15.00m, 12.50m),
            CurrentUnitRate: 14.50m,
            AverageRate: 14.75m
        );

        var mockReader = new Mock<IEnergyPricingReader>();
        mockReader.Setup(r => r.GetPricingDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pricingData);

        var handler = new GetEnergyPricingHandler(mockReader.Object);

        // Act
        var result = await handler.Handle(new GetEnergyPricing(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(14.50m, result.CurrentUnitRate);
        Assert.Equal(1450m, result.CurrentUnitRatePence);
        Assert.NotNull(result.CurrentPeriod);
        Assert.NotNull(result.NextCheapestPeriod);
        Assert.NotEmpty(result.Prices24H);
        Assert.Equal(2, result.Prices24H.Count);
    }

    [Fact]
    public async Task Handle_WithNoPricingData_ReturnsNull()
    {
        // Arrange
        var mockReader = new Mock<IEnergyPricingReader>();
        mockReader.Setup(r => r.GetPricingDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((EnergyPricingData?)null);

        var handler = new GetEnergyPricingHandler(mockReader.Object);

        // Act
        var result = await handler.Handle(new GetEnergyPricing(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_IdentifiesMinimumPrice()
    {
        // Arrange
        var pricingData = new EnergyPricingData(
            Prices: new List<EnergyPrice>
            {
                new(DateTime.UtcNow.AddHours(0), DateTime.UtcNow.AddHours(1), 20.00m, 16.67m),
                new(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 10.00m, 8.33m),
                new(DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), 15.00m, 12.50m),
            },
            CurrentPrice: new EnergyPrice(DateTime.UtcNow.AddHours(0), DateTime.UtcNow.AddHours(1), 20.00m, 16.67m),
            NextCheapestPeriod: new EnergyPrice(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 10.00m, 8.33m),
            CurrentUnitRate: 20.00m,
            AverageRate: 15.00m
        );

        var mockReader = new Mock<IEnergyPricingReader>();
        mockReader.Setup(r => r.GetPricingDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pricingData);

        var handler = new GetEnergyPricingHandler(mockReader.Object);

        // Act
        var result = await handler.Handle(new GetEnergyPricing(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var cheapestPrice = result.Prices24H.Single(p => p.IsCheapest);
        Assert.Equal(10.00m, cheapestPrice.UnitRateInclVat);
    }

    [Fact]
    public async Task Handle_ConvertsToPenceCorrectly()
    {
        // Arrange
        var pricingData = new EnergyPricingData(
            Prices: new List<EnergyPrice>
            {
                new(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, 12.34m, 10.28m),
            },
            CurrentPrice: new EnergyPrice(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, 12.34m, 10.28m),
            NextCheapestPeriod: null,
            CurrentUnitRate: 12.34m,
            AverageRate: 12.34m
        );

        var mockReader = new Mock<IEnergyPricingReader>();
        mockReader.Setup(r => r.GetPricingDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pricingData);

        var handler = new GetEnergyPricingHandler(mockReader.Object);

        // Act
        var result = await handler.Handle(new GetEnergyPricing(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(12.34m, result.CurrentUnitRate);
        Assert.Equal(1234m, result.CurrentUnitRatePence);
        Assert.Equal(12.34m, result.CurrentPeriod!.UnitRateInclVat);
        Assert.Equal(1234m, result.CurrentPeriod.UnitRateInclVatPence);
    }
}
