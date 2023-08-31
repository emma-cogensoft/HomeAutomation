using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryState;

public class DrainedState : BatteryState
{
    public override string Description => "Fully drained";
    
    public DrainedState(Percentage batteryPercentage, WattHours batteryCapacity) 
        : base(batteryPercentage, batteryCapacity) { }
}