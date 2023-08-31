using HomeAutomation.Domain.Battery.BatteryActivity;
using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(BatteryActivity))]
public class BatteryActivityTests
{
    [Test]
    public void BatteryActivity_WhenBatteryIsFullyCharged_ReturnsExpectedValues()
    {
        // Arrange
        var batteryPowerUsage = new Watt(0);
        var remainingBatteryCapacity = new WattHours(100);
        
    }
}