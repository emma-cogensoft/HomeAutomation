using System.Text.Json;
using System.Text.Json.Nodes;
using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain;
using HomeAutomation.Domain.Inverter;
using HomeAutomation.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace HomeAutomation.LocalInverter.InverterApiSettings;

public class LocalInverterSettingsDataReader : IInverterSettingsDataReader
{
    private readonly ILocalInverterApiAccessor _httpAccessor;
    private readonly LocalInverterApiSettingsOptions _options;

    public LocalInverterSettingsDataReader(ILocalInverterApiAccessor httpAccessor, IOptions<LocalInverterApiSettingsOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(httpAccessor);

        _options = options.Value;
        _httpAccessor = httpAccessor;
    }

    public async Task<CurrentInverterSettings> GetCurrentSettingsAsync(CancellationToken cancellationToken)
    {
        var uri = BuildApiUri();
        var body = BuildApiBody("ReadSetData");
        var response = await _httpAccessor.GetStringAsync(uri, body, cancellationToken);

        var inverterData = JsonSerializer.Deserialize<JsonArray>(response);
        if (inverterData == null) throw new LocalInverterApiException("Could not read inverter data from settings endpoint");
        if (inverterData.Count < 32) throw new LocalInverterApiException("Could not read inverter data from settings endpoint");

        return new CurrentInverterSettings
        {
            CurrentSettingType = MapSettings(GetCurrentlyEnabledSetting(inverterData)),
            SelfUseSettings = new CurrentInverterSettings.SelfUse(
                GetBoolDataItem(inverterData, DataItem.ChargeFromGridIsEnabled),
                GetIntDataItem(inverterData, DataItem.ChargeFromGridMaxBatteryPercent, -1),
                GetIntDataItem(inverterData, DataItem.ChargeFromGridMinAllowedBatteryPercent, -1)),
            FeedInPrioritySettings = new CurrentInverterSettings.FeedInPriority(
                GetIntDataItem(inverterData, DataItem.FeedInPrioritySoc, -1),
                GetIntDataItem(inverterData, DataItem.FeedInPriorityChargeBatteryTo, -1)
            ),
            ChargeAndDischargeSettings = new CurrentInverterSettings.ChargeAndDischarge(
                Period1: new CurrentInverterSettings.ChargeAndDischarge.Period(
                    GetBoolDataItem(inverterData, DataItem.ChargeAndDischargePeriod1IsEnabled),
                    GetTimeOfDayDataItem(inverterData, DataItem.ChargeAndDischargePeriod1ForcedChargePeriodStartTime),
                    GetTimeOfDayDataItem(inverterData, DataItem.ChargeAndDischargePeriod1ForcedChargePeriodEndTime),
                    GetTimeOfDayDataItem(inverterData,
                        DataItem.ChargeAndDischargePeriod1AllowedDischargePeriodStartTime),
                    GetTimeOfDayDataItem(inverterData, DataItem.ChargeAndDischargePeriod1AllowedDischargePeriodEndTime)
                ),
                Period2: new CurrentInverterSettings.ChargeAndDischarge.Period(
                    GetBoolDataItem(inverterData, DataItem.ChargeAndDischargePeriod2IsEnabled),
                    GetTimeOfDayDataItem(inverterData, DataItem.ChargeAndDischargePeriod2ForcedChargePeriodStartTime),
                    GetTimeOfDayDataItem(inverterData, DataItem.ChargeAndDischargePeriod2ForcedChargePeriodEndTime),
                    GetTimeOfDayDataItem(inverterData,
                        DataItem.ChargeAndDischargePeriod2AllowedDischargePeriodStartTime),
                    GetTimeOfDayDataItem(inverterData, DataItem.ChargeAndDischargePeriod2AllowedDischargePeriodEndTime)
                )
            ),
            TimeStamp = TimeProvider.UtcNow // TODO replace with timestamp from inverter
        };
    }

    private Uri BuildApiUri() => Uri.TryCreate(_options.RequestUri, UriKind.Absolute, out var uri)
        ? uri
        : throw new LocalInverterApiException("Could not build API URI");

    private string BuildApiBody(string requestName) =>
        _options.RequestBody.Replace("{RequestName}", requestName, StringComparison.OrdinalIgnoreCase);

    private static int GetIntDataItem(JsonArray data, DataItem itemIndex, int defaultValueIfNull = 0) =>
        data[(int)itemIndex]?.GetValue<int>() ?? defaultValueIfNull;

    private static bool GetBoolDataItem(JsonArray data, DataItem itemIndex) =>
        (data[(int)itemIndex]?.GetValue<int>() ?? 0) != 0;

    private static TimeOfDay GetTimeOfDayDataItem(JsonArray data, DataItem itemIndex) =>
        data[(int)itemIndex]?.GetValue<int>() ?? TimeOfDay.Empty;

    private static CurrentlyEnabledSetting GetCurrentlyEnabledSetting(JsonArray data) =>
        (CurrentlyEnabledSetting)(data[(int)DataItem.CurrentlyEnabledSetting]?.GetValue<int>() ?? 0);

    private static InverterSettingType MapSettings(CurrentlyEnabledSetting setting) => 
        setting switch
        {
            CurrentlyEnabledSetting.FeedInPriority => InverterSettingType.FeedInPriority,
            CurrentlyEnabledSetting.Manual => InverterSettingType.Manual,
            CurrentlyEnabledSetting.SelfUse => InverterSettingType.SelfUse,
            CurrentlyEnabledSetting.BackupMode => InverterSettingType.BackupMode,
            _ => throw new LocalInverterApiException("Could not read current inverter setting type")
        };
}