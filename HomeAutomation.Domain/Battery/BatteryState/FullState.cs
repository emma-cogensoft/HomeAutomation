using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryState;

public class FullState : BatteryState
{
    public override string Description => "Fully charged";
    
    public FullState(Percentage batteryPercentage, WattHours totalBatteryCapacity) 
        : base(batteryPercentage, totalBatteryCapacity) { }
}