using HomeAutomation.Domain.Battery.BatteryActivity;
using HomeAutomation.Domain.Battery.BatteryState;
using NUnit.Framework.Internal;

namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(BatteryActivityFactory))]
public class BatteryActivityFactoryTests
{
    [Test]
    [TestCase(-1_000)]
    [TestCase(-1)]
    [TestCase(0)]
    public void CreateActivity_WhenBatteryIsPartiallyFullAndPowerUsageIsNegativeOr0_CreatesDischargingActivity(int batteryPowerUsage)
    {
        // Arrange
        var batteryState = CreatePartiallyFullBatteryState();

        // Act
        var batteryActivity = batteryState.CreateActivity(batteryPowerUsage);

        // Assert
        Assert.That(batteryActivity, Is.InstanceOf<DischargingActivity>());
    }
    
    [Test]
    [TestCase(1_000)]
    [TestCase(1)]
    public void CreateActivity_WhenBatteryIsPartiallyFullAndPowerUsageIsPositive_CreatesDischargingActivity(int batteryPowerUsage)
    {
        // Arrange
        var batteryState = CreatePartiallyFullBatteryState();

        // Act
        var batteryActivity = batteryState.CreateActivity(batteryPowerUsage);

        // Assert
        Assert.That(batteryActivity, Is.InstanceOf<ChargingActivity>());
    }
    
    [Test]
    [TestCase(-1_000)]
    [TestCase(-1)]
    [TestCase(0)]
    public void CreateActivity_WhenBatteryIsFullAndPowerUsageIsNegativeOr0_CreatesDischargingActivity(int batteryPowerUsage)
    {
        // Arrange
        var batteryState = CreateFullBatteryState();

        // Act
        var batteryActivity = batteryState.CreateActivity(batteryPowerUsage);

        // Assert
        Assert.That(batteryActivity, Is.InstanceOf<DischargingActivity>());
    }
    
    [Test]
    [TestCase(1_000)]
    [TestCase(1)]
    public void CreateActivity_WhenBatteryIsFullAndPowerUsageIsPositive_CreatesChargingActivity(int batteryPowerUsage)
    {
        // Arrange
        var batteryState = CreateFullBatteryState();

        // Act
        var batteryActivity = batteryState.CreateActivity(batteryPowerUsage);

        // Assert
        Assert.That(batteryActivity, Is.InstanceOf<ChargingActivity>());
    }
    
    [Test]
    [TestCase(-1_000)]
    [TestCase(-1)]
    [TestCase(0)]
    public void CreateActivity_WhenBatteryIsDrainedAndPowerUsageIsNegativeOr0_CreatesDischargingActivity(int batteryPowerUsage)
    {
        // Arrange
        var batteryState = CreateDrainedBatteryState();

        // Act
        var batteryActivity = batteryState.CreateActivity(batteryPowerUsage);

        // Assert
        Assert.That(batteryActivity, Is.InstanceOf<DischargingActivity>());
    }
    
    [Test]
    [TestCase(1_000)]
    [TestCase(1)]
    public void CreateActivity_WhenBatteryIsDrainedAndPowerUsageIsPositive_CreatesChargingActivity(int batteryPowerUsage)
    {
        // Arrange
        var batteryState = CreateDrainedBatteryState();

        // Act
        var batteryActivity = batteryState.CreateActivity(batteryPowerUsage);

        // Assert
        Assert.That(batteryActivity, Is.InstanceOf<ChargingActivity>());
    }
    
    private static PartiallyFullState CreatePartiallyFullBatteryState()
    {
        var randomizer = new Randomizer();

        return new PartiallyFullState(randomizer.NextShort(11, 98), randomizer.NextShort(1_000, 10_000));
    }

    private static FullState CreateFullBatteryState()
    {
        var randomizer = new Randomizer();

        return new FullState(randomizer.NextShort(99, 100), randomizer.NextShort(1_000, 10_000));
    }
    
    private static DrainedState CreateDrainedBatteryState()
    {
        var randomizer = new Randomizer();

        return new DrainedState(randomizer.NextShort(1, 10), randomizer.NextShort(1_000, 10_000));
    }
}