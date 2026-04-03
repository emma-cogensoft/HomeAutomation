using HomeAutomation.Domain.Battery.BatteryActivity;
using HomeAutomation.Domain.Battery.BatteryState;
using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery;

public record BatteryInfo
{
    public WattHours BatteryCapacity { get; init; }
    public BatteryState.BatteryState BatteryState { get; init; }
    public BatteryActivity.BatteryActivity BatteryActivity { get; init; }

    public BatteryInfo(Watt batteryPowerUsage, Percentage batteryPowerPercentage, WattHours capacityInWh)
    {
        BatteryCapacity = capacityInWh;
        BatteryState = BatteryStateFactory.CreateState(batteryPowerPercentage, BatteryCapacity);
        BatteryActivity = BatteryState.CreateActivity(batteryPowerUsage);
    }
}