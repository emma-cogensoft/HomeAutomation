using HomeAutomation.Domain.Battery.BatteryState;
using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(FullState))]
public class FullStateTests
{
    [Test]
    [TestCase(100, 5_000, 90, 0, 4_500, 5_000)]
    [TestCase(100, 10_000, 90, 0, 9_000, 10_000)]
    [TestCase(99, 5_000, 89, 1, 4_450, 4_950)]
    [TestCase(99, 10_000, 89, 1, 8_900, 9_900)]
    public void FullState_ReturnsExpectedValues(
        int batteryPercentCharged,
        int totalBatteryCapacity,
        int expectedAvailablePercentCharged,
        int expectedPercentUncharged,
        int expectedAvailableChargeInBattery,
        int expectedTotalChargeInBattery)
    {
        // Act
        var sut = new FullState(batteryPercentCharged, totalBatteryCapacity);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.TotalChargeInBattery, Is.EqualTo((WattHours)expectedTotalChargeInBattery));
            Assert.That(sut.PercentageCharged, Is.EqualTo((Percentage)batteryPercentCharged));
            Assert.That(sut.PercentageUncharged, Is.EqualTo((Percentage)expectedPercentUncharged));
            Assert.That(sut.Description, Is.EqualTo("Fully charged"));
            Assert.That(sut.AvailablePercentageCharged, Is.EqualTo((Percentage)expectedAvailablePercentCharged));
            Assert.That(sut.AvailableChargeInBattery, Is.EqualTo((WattHours)expectedAvailableChargeInBattery));
            Assert.That(sut.TimeStamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        });
    }
    
    [Test]
    [TestCase(-1)]
    [TestCase(101)]
    public void FullState_WhenPercentageOutOfRange_ThrowsException(int batteryPercentCharged)
    {
        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new FullState(batteryPercentCharged, 1000));

        // Assert
        Assert.That(exception.Message, Is.EqualTo($"Percentage value must be between 0 and 100 (Parameter 'value'){Environment.NewLine}Actual value was {batteryPercentCharged}."));
    }
    
    [Test]
    [TestCase(-1)]
    public void FullState_WhenBatteryCapacityOutOfRange_ThrowsException(int totalBatteryCapacity)
    {
        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new FullState(100, totalBatteryCapacity));

        // Assert
        Assert.That(exception.Message, Is.EqualTo($"WattHours value must be greater than 0 (Parameter 'value'){Environment.NewLine}Actual value was {totalBatteryCapacity}."));
    }
}