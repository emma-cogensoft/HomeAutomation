using System.Text.Json;
using HomeAutomation.Application.Services.Inverter;
using Microsoft.Extensions.Options;

namespace HomeAutomation.CloudInverter.InverterApiRealTimeData;

public class CloudInverterRealtimeDataReader : IInverterRealtimeDataReader
{
    private readonly ICloudInverterApiAccessor _httpAccessor;
    private readonly CloudInverterApiOptions _options;

    public CloudInverterRealtimeDataReader(ICloudInverterApiAccessor httpAccessor,
        IOptions<CloudInverterApiOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(httpAccessor);

        _options = options.Value;
        _httpAccessor = httpAccessor;
    }

    public async Task<InverterRealtimeData> GetInverterRealtimeDataAsync(CancellationToken cancellationToken)
    {
        var uri = TryBuildApiUri(_options.RequestTemplateUri, "getRealtimeInfo");
        var response = await _httpAccessor.GetJsonAsync(uri, cancellationToken);
        var inverterData = response.Deserialize<CloudInverterRealtimeDataResponse>();
        if (inverterData == null) throw new CloudInverterException("Could not read inverter data");

        return new InverterRealtimeData
        {
            BatteryPercentage = inverterData.Result.Soc ?? -1,
            BatteryPowerUsage = inverterData.Result.BatPower ?? -1,
            FeedIn = inverterData.Result.FeedInPower ?? -1,
            HomeUsage = inverterData.Result.FeedInPower + inverterData.Result.PowerDc2 ?? -1,
            SolarInput = inverterData.Result.PowerDc2 ?? -1,
            TimeStamp = TryConvertStringDateTime(inverterData.Result.UploadTime ?? string.Empty),
            Source = "CloudInverter"
        };
    }

    private static Uri TryBuildApiUri(string maybeUri, string requestName)
    {
        var uriToTry = maybeUri.Replace("{requestName}", requestName, StringComparison.OrdinalIgnoreCase);
        
        return Uri.TryCreate(uriToTry, UriKind.Absolute, out var uri)
            ? uri
            : throw new CloudInverterException("Could not build API URI");
    }
    
    private static DateTime TryConvertStringDateTime(string dateToTry) =>
        DateTime.TryParse(dateToTry, out var date)
            ? date
            : throw new CloudInverterException("Could not convert date from inverter");
}
