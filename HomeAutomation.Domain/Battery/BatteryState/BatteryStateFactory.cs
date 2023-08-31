using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryState;

public static class BatteryStateFactory
{
    public static BatteryState CreateState(Percentage batteryPercentage, WattHours batteryCapacity)
    {
        return batteryPercentage.Value switch
        {
            <= BatteryState.MinimumAllowedPercentageCharged => new DrainedState(batteryPercentage, batteryCapacity),
            >= BatteryState.FullBatteryThresholdPercentage => new FullState(batteryPercentage, batteryCapacity),
            _ => new PartiallyFullState(batteryPercentage, batteryCapacity)
        };
    }
}