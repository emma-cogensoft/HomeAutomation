namespace HomeAutomation.LocalInverter.InverterApiSettings;

internal enum DataItem
{
    CurrentlyEnabledSetting = 27,

    ChargeFromGridMinAllowedBatteryPercent = 28,
    ChargeFromGridIsEnabled = 29,
    ChargeFromGridMaxBatteryPercent = 30,

    FeedInPrioritySoc = 31,
    FeedInPriorityChargeBatteryTo = 32,

    ChargeAndDischargePeriod1IsEnabled = 35,
    ChargeAndDischargePeriod1ForcedChargePeriodStartTime = 36,
    ChargeAndDischargePeriod1ForcedChargePeriodEndTime = 37,
    ChargeAndDischargePeriod1AllowedDischargePeriodStartTime = 38,
    ChargeAndDischargePeriod1AllowedDischargePeriodEndTime = 39,

    ChargeAndDischargePeriod2IsEnabled = 40,
    ChargeAndDischargePeriod2ForcedChargePeriodStartTime = 41,
    ChargeAndDischargePeriod2ForcedChargePeriodEndTime = 42,
    ChargeAndDischargePeriod2AllowedDischargePeriodStartTime = 43,
    ChargeAndDischargePeriod2AllowedDischargePeriodEndTime = 44
}