using HomeAutomation.Domain.Battery.BatteryActivity;
using HomeAutomation.Domain.Battery.BatteryState;
using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(ChargingActivity))]
public class ChargingTests
{
    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    public void WhenBatteryIsFull_HasExpectedValues(int powerUsage)
    {
        // Arrange
        var batteryState = new FullState(100, 1000);
        
        // Act
        var sut = new ChargingActivity(powerUsage, batteryState);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.Description, Is.EqualTo("Charging"));
            Assert.That(sut.TimeToComplete, Is.EqualTo(0));
            Assert.That(sut.BatteryPowerUsage, Is.EqualTo((Watt)powerUsage));
            Assert.That(sut.ChargeAmountUntilActivityComplete, Is.EqualTo((WattHours)0));
            Assert.That(sut.TimeStamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        });
    }

    [Test]
    [TestCase(100, 50, 1000, 5, 500)]
    [TestCase(1000, 50, 1000, 0, 500)]
    [TestCase(100, 50, 2000, 10, 1000)]
    [TestCase(1000, 50, 2000, 1, 1000)]
    public void WhenBatteryIsHalfFull_HasExpectedValues(int powerUsage, int batteryChargedPercentage, int batteryCapacity, 
        int expectedTimeToComplete, int expectedChargeAmountUntilActivityComplete)
    {
        // Arrange
        var batteryState = new PartiallyFullState(batteryChargedPercentage, batteryCapacity);
        
        // Act
        var sut = new ChargingActivity(powerUsage, batteryState);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.Description, Is.EqualTo("Charging"));
            Assert.That(sut.TimeToComplete, Is.EqualTo(expectedTimeToComplete));
            Assert.That(sut.BatteryPowerUsage, Is.EqualTo((Watt)powerUsage));
            Assert.That(sut.ChargeAmountUntilActivityComplete, Is.EqualTo((WattHours)expectedChargeAmountUntilActivityComplete));
            Assert.That(sut.TimeStamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        });
    }
    
    [Test]
    [TestCase(100, 0, 1000, 10, 1000)]
    [TestCase(1000, 0, 1000, 1, 1000)]
    [TestCase(100, 0, 2000, 20, 2000)]
    [TestCase(1000, 0, 2000, 2, 2000)]
    
    [TestCase(100, 10, 1000, 9, 900)]
    [TestCase(1000, 10, 1000, 0, 900)]
    [TestCase(100, 10, 2000, 18, 1800)]
    [TestCase(1000, 10, 2000, 1, 1800)]
    public void WhenBatteryIsEmpty_HasExpectedValues(int powerUsage, int batteryChargedPercentage, int batteryCapacity, 
        int expectedTimeToComplete, int expectedChargeAmountUntilActivityComplete)
    {
        // Arrange
        var batteryState = new DrainedState(batteryChargedPercentage, batteryCapacity);

        // Act
        var sut = new ChargingActivity(powerUsage, batteryState);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.Description, Is.EqualTo("Charging"));
            Assert.That(sut.TimeToComplete, Is.EqualTo(expectedTimeToComplete));
            Assert.That(sut.BatteryPowerUsage, Is.EqualTo((Watt)powerUsage));
            Assert.That(sut.ChargeAmountUntilActivityComplete, Is.EqualTo((WattHours)expectedChargeAmountUntilActivityComplete));
            Assert.That(sut.TimeStamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        });
    }
}