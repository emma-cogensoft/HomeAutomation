using HomeAutomation.Domain.Battery;
using HomeAutomation.Domain.Battery.BatteryActivity;
using HomeAutomation.Domain.Battery.BatteryState;

namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(BatteryInfo))]
public class BatteryInfoTests
{
    [Test]
    [TestCase(100, 100)]
    [TestCase(100, 99)]
    [TestCase(1000, 100)]
    [TestCase(1000, 99)]
    [TestCase(1, 100)]
    [TestCase(1, 99)]
    public void Create_WhenBatteryFullAndPowerUsageIsPositive_HasFullStateAndChargingActivity(int batteryPowerUsage, int batteryPowerPercentage)
    {
        // Act
        var sut = new BatteryInfo(batteryPowerUsage, batteryPowerPercentage);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.BatteryState, Is.TypeOf<FullState>());
            Assert.That(sut.BatteryActivity, Is.TypeOf<ChargingActivity>());
        });
    }
   
    [Test]
    [TestCase(100, 10)]
    [TestCase(1000, 10)]
    [TestCase(1, 10)]
    public void Create_WhenBatteryEmptyAndPowerUsageIsPositive_HasEmptyStateAndChargingActivity(int batteryPowerUsage, int batteryPowerPercentage)
    {
        // Act
        var sut = new BatteryInfo(batteryPowerUsage, batteryPowerPercentage);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.BatteryState, Is.TypeOf<DrainedState>());
            Assert.That(sut.BatteryActivity, Is.TypeOf<ChargingActivity>());
        });
    }
    
    [Test]
    [TestCase(100, 50)]
    [TestCase(1000, 50)]
    [TestCase(1, 50)]
    public void Create_WhenBatteryHalfFullAndPowerUsageIsPositive_HasHalfFullStateAndChargingActivity(int batteryPowerUsage, int batteryPowerPercentage)
    {
        // Act
        var sut = new BatteryInfo(batteryPowerUsage, batteryPowerPercentage);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.BatteryState, Is.TypeOf<PartiallyFullState>());
            Assert.That(sut.BatteryActivity, Is.TypeOf<ChargingActivity>());
        });
    }

    [Test]
    [TestCase(-100, 100)]
    [TestCase(-100, 99)]
    [TestCase(-1000, 100)]
    [TestCase(-1000, 99)]
    [TestCase(-1, 100)]
    [TestCase(-1, 99)]
    public void Create_WhenBatteryFullAndPowerUsageIsNegative_HasFullStateAndDischargingActivity(int batteryPowerUsage, int batteryPowerPercentage)
    {
        // Act
        var sut = new BatteryInfo(batteryPowerUsage, batteryPowerPercentage);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.BatteryState, Is.TypeOf<FullState>());
            Assert.That(sut.BatteryActivity, Is.TypeOf<DischargingActivity>());
        });
    }
    
    [Test]
    [TestCase(-100, 10)]
    [TestCase(-1000, 10)]
    [TestCase(-1, 10)]
    public void Create_WhenBatteryEmptyAndPowerUsageIsNegative_HasEmptyStateAndDischargingActivity(int batteryPowerUsage, int batteryPowerPercentage)
    {
        // Act
        var sut = new BatteryInfo(batteryPowerUsage, batteryPowerPercentage);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.BatteryState, Is.TypeOf<DrainedState>());
            Assert.That(sut.BatteryActivity, Is.TypeOf<DischargingActivity>());
        });
    }
    
    [Test]
    [TestCase(-100, 50)]
    [TestCase(-1000, 50)]
    [TestCase(-1, 50)]
    public void Create_WhenBatteryHalfFullAndPowerUsageIsNegative_HasHalfFullStateAndDischargingActivity(int batteryPowerUsage, int batteryPowerPercentage)
    {
        // Act
        var sut = new BatteryInfo(batteryPowerUsage, batteryPowerPercentage);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.BatteryState, Is.TypeOf<PartiallyFullState>());
            Assert.That(sut.BatteryActivity, Is.TypeOf<DischargingActivity>());
        });
    }
}