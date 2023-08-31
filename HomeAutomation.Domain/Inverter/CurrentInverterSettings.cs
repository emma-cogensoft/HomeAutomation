using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Inverter;

public class CurrentInverterSettings
{
    public InverterWorkType CurrentWorkType { get; init; }
    public string CurrentWorkTypeName => CurrentWorkType.ToString();
    
    public SelfUse SelfUseSettings { get; init; } = null!;
    public FeedInPriority FeedInPrioritySettings { get; init; } = null!;
    public ChargeAndDischarge ChargeAndDischargeSettings { get; init; } = null!;
    public required DateTime TimeStamp { get; init; }

    public record SelfUse(bool IsChargeFromGridEnabled, int MinimumAllowedBatteryPercentage, int MaximumBatteryPercentage);
    
    public record FeedInPriority(Percentage MinimumAllowedBatteryPercentage, Percentage ChargeBatteryToBatteryPercentage);

    public record ChargeAndDischarge(ChargeAndDischarge.Period Period1, ChargeAndDischarge.Period Period2)
    {
        public record Period(bool IsPeriodEnabled, TimeOfDay ForcedChargePeriodStartTime, TimeOfDay ForcedChargePeriodEndTime, TimeOfDay AllowedDischargePeriodStartTime, TimeOfDay AllowedDischargePeriodEndTime);
    }
}