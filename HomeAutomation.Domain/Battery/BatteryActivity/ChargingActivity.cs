using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryActivity;

public class ChargingActivity : BatteryActivity
{
    public override string Description => "Charging";

    public ChargingActivity(Watt batteryPowerUsage, BatteryState.BatteryState batteryState)
        : base(batteryPowerUsage, batteryState.RemainingBatteryCapacity)
    {
    }
}