using System.Text.Json;
using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain;
using HomeAutomation.Domain.Inverter;
using HomeAutomation.LocalInverter.ApiAccessor;
using Microsoft.Extensions.Options;

namespace HomeAutomation.LocalInverter.RealTimeData;

public class LocalInverterRealtimeDataReader : IInverterRealtimeDataReader
{
    private readonly ILocalInverterApiAccessor _httpAccessor;
    private readonly LocalInverterApiOptions _options;

    public LocalInverterRealtimeDataReader(ILocalInverterApiAccessor httpAccessor,
        IOptions<LocalInverterApiOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(httpAccessor);

        _options = options.Value;
        _httpAccessor = httpAccessor;
    }

    public async Task<InverterRealtimeData> GetInverterRealtimeDataAsync(CancellationToken cancellationToken)
    {
        var uri = TryBuildApiUri(_options.RequestUri);
        var body = BuildApiBody("ReadRealTimeData");
        var response = await _httpAccessor.GetJsonAsync(uri, body, cancellationToken);
        var batteryData = response.Deserialize<LocalInverterApiResponse>();
        if (batteryData == null) throw new LocalInverterApiException("Could not read battery data");

        return new InverterRealtimeData
        {
            BatteryPercentage = GetDataItem(batteryData, DataItem.BatteryPercentage, -1),
            BatteryPowerUsage = GetDataItem(batteryData, DataItem.BatteryPowerUsage) > 40000
                ? -(65535 - GetDataItem(batteryData, DataItem.BatteryPowerUsage))
                : GetDataItem(batteryData, DataItem.BatteryPowerUsage),
            FeedIn = GetDataItem(batteryData, DataItem.FeedIn, -1),
            HomeUsage = GetDataItem(batteryData, DataItem.HomeUsage, -1),
            SolarInput = GetDataItem(batteryData, DataItem.SolarInput, -1),
            TimeStamp = TimeProvider.UtcNow,
            Source = "LocalInverter"
        };
    }

    private static Uri TryBuildApiUri(string maybeUri) => Uri.TryCreate(maybeUri, UriKind.Absolute, out var uri)
        ? uri
        : throw new LocalInverterApiException("Could not build API URI");

    private string BuildApiBody(string requestName) =>
        _options.RequestBody.Replace("{RequestName}", requestName, StringComparison.OrdinalIgnoreCase);

    private static int GetDataItem(LocalInverterApiResponse data, DataItem itemIndex, int defaultValueIfNull = 0) =>
        data.Data[(int)itemIndex]?.GetValue<int>() ?? defaultValueIfNull;

    private enum DataItem
    {
        HomeUsage = 2,
        SolarInput = 9,
        BatteryPowerUsage = 16,
        BatteryPercentage = 18,
        FeedIn = 23
    }
}