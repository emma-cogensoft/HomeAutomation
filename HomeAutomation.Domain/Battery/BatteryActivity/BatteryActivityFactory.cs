using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryActivity;

public static class BatteryActivityFactory
{
    public static BatteryActivity CreateActivity(this BatteryState.BatteryState batteryState, Watt batteryPowerUsage)
        => batteryPowerUsage > 0 
            ? new ChargingActivity(batteryPowerUsage, batteryState) 
            : new DischargingActivity(batteryPowerUsage, batteryState);
}