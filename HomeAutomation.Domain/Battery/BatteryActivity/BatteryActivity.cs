using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryActivity;

public abstract class BatteryActivity
{
    private const int MaxTimeToComplete = 24;
    
    public DateTime TimeStamp { get; init; }
    public Watt BatteryPowerUsage { get; init; }
    public WattHours ChargeAmountUntilActivityComplete { get; init; }
    
    public abstract string Description { get; }
    public int TimeToComplete => 
        ChargeAmountUntilActivityComplete == 0
            ? 0
            : BatteryPowerUsage == 0
                ? MaxTimeToComplete
                : ChargeAmountUntilActivityComplete / Math.Abs(BatteryPowerUsage);

    protected BatteryActivity(Watt batteryPowerUsage, WattHours chargeAmountUntilActivityComplete)
    {
        BatteryPowerUsage = batteryPowerUsage;
        ChargeAmountUntilActivityComplete = chargeAmountUntilActivityComplete;
        TimeStamp = TimeProvider.UtcNow;
    }
}