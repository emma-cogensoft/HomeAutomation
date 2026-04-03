using System.Text.Json;
using System.Text.Json.Nodes;
using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain;
using HomeAutomation.Domain.Inverter;
using HomeAutomation.Domain.ValueObjects;
using HomeAutomation.LocalInverter.ApiAccessor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomeAutomation.LocalInverter.InverterSettings;

public class LocalInverterSettingsDataReader : IInverterSettingsDataReader
{
    private readonly ILocalInverterApiAccessor _httpAccessor;
    private readonly LocalInverterApiOptions _options;
    private readonly ILogger<LocalInverterSettingsDataReader> _logger;

    public LocalInverterSettingsDataReader(ILocalInverterApiAccessor httpAccessor,
        IOptions<LocalInverterApiOptions> options,
        ILogger<LocalInverterSettingsDataReader> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(httpAccessor);

        _options = options.Value;
        _httpAccessor = httpAccessor;
        _logger = logger;
    }

    public async Task<CurrentInverterSettings> GetCurrentSettingsAsync(CancellationToken cancellationToken)
    {
        var uri = BuildApiUri();
        var body = BuildApiBody("ReadSetData");
        _logger.LogDebug("Requesting local inverter settings from {Uri}", uri);
        var response = await _httpAccessor.GetStringAsync(uri, body, cancellationToken);

        var inverterData = JsonSerializer.Deserialize<JsonArray>(response);
        if (inverterData == null) throw new LocalInverterApiException("Could not read inverter data from settings endpoint");
        if (inverterData.Count < 32) throw new LocalInverterApiException("Could not read inverter data from settings endpoint");

        var workType = MapSettings(GetCurrentlySelectedWorkType(inverterData));
        _logger.LogDebug("Local inverter settings retrieved: current work type {WorkType}", workType);

        return new CurrentInverterSettings
        {
            CurrentWorkType = workType,
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
            TimeStamp = HomeAutomation.Domain.TimeProvider.UtcNow // TODO replace with timestamp from inverter
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

    private static SelectedWorkType GetCurrentlySelectedWorkType(JsonArray data) =>
        (SelectedWorkType)(data[(int)DataItem.CurrentlyEnabledSetting]?.GetValue<int>() ?? 0);

    private static InverterWorkType MapSettings(SelectedWorkType setting) => 
        setting switch
        {
            SelectedWorkType.FeedInPriority => InverterWorkType.FeedInPriority,
            SelectedWorkType.Manual => InverterWorkType.Manual,
            SelectedWorkType.SelfUse => InverterWorkType.SelfUse,
            SelectedWorkType.BackupMode => InverterWorkType.BackupMode,
            _ => throw new LocalInverterApiException("Could not read current inverter setting type")
        };
}