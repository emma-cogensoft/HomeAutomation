using HomeAutomation.Domain.Battery.BatteryState;
using NUnit.Framework.Internal;

namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(BatteryStateFactory))]
public class BatteryStateFactoryTests
{
    [Test]
    [TestCase(100)]
    [TestCase(99)]
    public void CreateState_WhenBatteryIsFull_CreatesFullState(int batteryPercentage)
    {
        // Arrange
        var randomizer = new Randomizer();
        var batteryCapacity = randomizer.NextShort(1_000, 10_000);

        // Act
        var batteryState = BatteryStateFactory.CreateState(batteryPercentage, batteryCapacity);

        // Assert
        Assert.That(batteryState, Is.InstanceOf<FullState>());
    }
    
    [Test]
    [TestCase(10)]
    [TestCase(1)]
    public void CreateState_WhenBatteryIsDrained_CreatesDrainedState(int batteryPercentage)
    {
        // Arrange
        var randomizer = new Randomizer();
        var batteryCapacity = randomizer.NextShort(1_000, 10_000);

        // Act
        var batteryState = BatteryStateFactory.CreateState(batteryPercentage, batteryCapacity);

        // Assert
        Assert.That(batteryState, Is.InstanceOf<DrainedState>());
    }
    
    [Test]
    [TestCase(50)]
    [TestCase(11)]
    public void CreateState_WhenBatteryIsPartiallyFull_CreatesPartiallyFullState(int batteryPercentage)
    {
        // Arrange
        var randomizer = new Randomizer();
        var batteryCapacity = randomizer.NextShort(1_000, 10_000);

        // Act
        var batteryState = BatteryStateFactory.CreateState(batteryPercentage, batteryCapacity);

        // Assert
        Assert.That(batteryState, Is.InstanceOf<PartiallyFullState>());
    }
    
    [Test]
    [TestCase(-1)]
    [TestCase(101)]
    public void CreateState_WhenBatteryPercentageOutOfRange_ThrowsException(int batteryPercentage)
    {
        // Arrange
        var randomizer = new Randomizer();
        var batteryCapacity = randomizer.NextShort(1_000, 10_000);

        // Act
        void Sut() => BatteryStateFactory.CreateState(batteryPercentage, batteryCapacity);
        
        // Assert
        Assert.That(Sut, Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    
    [Test]
    [TestCase(-1)]
    public void CreateState_WhenBatteryCapacityOutOfRange_ThrowsException(int batteryCapacity)
    {
        // Arrange
        var randomizer = new Randomizer();
        var batteryPercentage = randomizer.NextShort(0, 100);

        // Act
        void Sut() => BatteryStateFactory.CreateState(batteryPercentage, batteryCapacity);
        
        // Assert
        Assert.That(Sut, Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}