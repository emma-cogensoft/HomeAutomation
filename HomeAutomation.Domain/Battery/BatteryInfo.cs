using HomeAutomation.Domain.Battery.BatteryActivity;
using HomeAutomation.Domain.Battery.BatteryState;
using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery;

public record BatteryInfo
{
    public WattHours BatteryCapacity => 5800;
    public BatteryState.BatteryState BatteryState { get; init; }
    public BatteryActivity.BatteryActivity BatteryActivity { get; init; }

    public BatteryInfo(Watt batteryPowerUsage, Percentage batteryPowerPercentage)
    {
        BatteryState = BatteryStateFactory.CreateState(batteryPowerPercentage, BatteryCapacity);
        BatteryActivity = BatteryState.CreateActivity(batteryPowerUsage);
    }
}