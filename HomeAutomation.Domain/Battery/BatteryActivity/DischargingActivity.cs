using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryActivity;

public class DischargingActivity : BatteryActivity
{
    public override string Description => "Discharging";

    public DischargingActivity(Watt batteryPowerUsage, BatteryState.BatteryState batteryState) 
        : base(batteryPowerUsage, batteryState.AvailableChargeInBattery)
    {
    }
}