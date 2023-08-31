using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Battery.BatteryState;

public abstract class BatteryState
{
    public const int MinimumAllowedPercentageCharged = 10;
    public const int FullBatteryThresholdPercentage = 99;

    public abstract string Description { get; }
    
    public DateTime TimeStamp { get; init; }
    
    public Percentage PercentageCharged { get; init; }

    public Percentage PercentageUncharged => 100 - PercentageCharged;

    public Percentage AvailablePercentageCharged =>
        PercentageCharged > MinimumAllowedPercentageCharged ? PercentageCharged - MinimumAllowedPercentageCharged : 0;

    private WattHours TotalBatteryCapacity { get; init; }

    public WattHours TotalChargeInBattery =>
        PercentageCharged == 0 ? 0 : TotalBatteryCapacity * PercentageCharged / 100;

    public WattHours AvailableChargeInBattery =>
        AvailablePercentageCharged == 0 ? 0 : TotalBatteryCapacity * AvailablePercentageCharged / 100;

    public WattHours RemainingBatteryCapacity => TotalBatteryCapacity * PercentageUncharged / 100;

    protected BatteryState(Percentage percentageCharged, WattHours totalBatteryCapacity)
    {
        PercentageCharged = percentageCharged;
        TotalBatteryCapacity = totalBatteryCapacity;
        TimeStamp = TimeProvider.UtcNow;
    }
}