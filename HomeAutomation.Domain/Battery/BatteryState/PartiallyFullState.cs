using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryState;

public class PartiallyFullState : BatteryState
{
    public override string Description => "Partially full";
    
    public PartiallyFullState(Percentage batteryPercentage, WattHours batteryCapacity) 
        : base(batteryPercentage, batteryCapacity) { }

}