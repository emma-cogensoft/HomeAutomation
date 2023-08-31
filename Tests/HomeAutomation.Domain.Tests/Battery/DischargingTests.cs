using HomeAutomation.Domain.Battery.BatteryActivity;
using HomeAutomation.Domain.Battery.BatteryState;
using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(DischargingActivity))]
public class DischargingTests
{
    [Test]
    [TestCase(-100, 9)]
    [TestCase(-1000, 0)]
    public void WhenBatteryIsFull_HasExpectedValues(int powerUsage, int expectedTimeToComplete)
    {
        // Arrange
        var batteryState = new FullState(100, 1000);

        // Act
        var sut = new DischargingActivity(powerUsage, batteryState);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.Description, Is.EqualTo("Discharging"));
            Assert.That(sut.TimeToComplete, Is.EqualTo(expectedTimeToComplete));
            Assert.That(sut.BatteryPowerUsage, Is.EqualTo((Watt)powerUsage));
            Assert.That(sut.ChargeAmountUntilActivityComplete, Is.EqualTo((WattHours)900));
            Assert.That(sut.TimeStamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        });
    }

    [Test]
    [TestCase(-100, 50, 1000, 4, 400)]
    [TestCase(-1000, 50, 1000, 0, 400)]
    [TestCase(-100, 50, 2000, 8, 800)]
    [TestCase(-1000, 50, 2000, 0, 800)]
    public void WhenBatteryIsHalfFull_HasExpectedValues(int powerUsage, int batteryChargedPercentage, int batteryCapacity, 
        int expectedTimeToComplete, int expectedChargeAmountUntilActivityComplete)
    {
        // Arrange
        var batteryState = new PartiallyFullState(batteryChargedPercentage, batteryCapacity);
        
        // Act
        var sut = new DischargingActivity(powerUsage, batteryState);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.Description, Is.EqualTo("Discharging"));
            Assert.That(sut.TimeToComplete, Is.EqualTo(expectedTimeToComplete));
            Assert.That(sut.BatteryPowerUsage, Is.EqualTo((Watt)powerUsage));
            Assert.That(sut.ChargeAmountUntilActivityComplete, Is.EqualTo((WattHours)expectedChargeAmountUntilActivityComplete));
            Assert.That(sut.TimeStamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        });
    }
    
    [Test]
    [TestCase(-100, 0, 1000, 0, 0)]
    [TestCase(-1000, 0, 1000, 0, 0)]
    [TestCase(-100, 0, 2000, 0, 0)]
    [TestCase(-1000, 0, 2000, 0, 0)]
    
    [TestCase(-100, 10, 1000, 0, 0)]
    [TestCase(-1000, 10, 1000, 0, 0)]
    [TestCase(-100, 10, 2000, 0, 0)]
    [TestCase(-1000, 10, 2000, 0, 0)]
    public void WhenBatteryIsEmpty_HasExpectedValues(int powerUsage, int batteryChargedPercentage, int batteryCapacity, 
        int expectedTimeToComplete, int expectedChargeAmountUntilActivityComplete)
    {
        // Arrange
        var batteryState = new DrainedState(batteryChargedPercentage, batteryCapacity);
        
        // Act
        var sut = new DischargingActivity(powerUsage, batteryState);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.Description, Is.EqualTo("Discharging"));
            Assert.That(sut.TimeToComplete, Is.EqualTo(expectedTimeToComplete));
            Assert.That(sut.BatteryPowerUsage, Is.EqualTo((Watt)powerUsage));
            Assert.That(sut.ChargeAmountUntilActivityComplete, Is.EqualTo((WattHours)expectedChargeAmountUntilActivityComplete));
            Assert.That(sut.TimeStamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        });
    }
}