using System.Text.Json.Serialization;

namespace HomeAutomation.CloudInverter.InverterApiRealTimeData;

internal class CloudInverterRealtimeDataResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("exception")] public string Exception { get; set; } = string.Empty;
    [JsonPropertyName("result")] public InverterData Result { get; set; } = new();
    [JsonPropertyName("code")] public int Code { get; set; }
}