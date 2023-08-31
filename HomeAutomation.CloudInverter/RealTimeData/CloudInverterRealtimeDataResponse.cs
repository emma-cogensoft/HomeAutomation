using System.Text.Json.Serialization;

namespace HomeAutomation.CloudInverter.RealTimeData;

internal class CloudInverterRealtimeDataResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("exception")] public string Exception { get; set; } = string.Empty;
    [JsonPropertyName("result")] public CloudInverterData Result { get; set; } = new();
    [JsonPropertyName("code")] public int Code { get; set; }
    
    internal class CloudInverterData
    {
        [JsonPropertyName("inverterSN")] public string? InverterSn { get; set; }
        [JsonPropertyName("sn")] public string? Sn { get; set; }
        [JsonPropertyName("acpower")] public double? AcPower { get; set; }
        [JsonPropertyName("yieldtoday")] public double? YieldToday { get; set; }
        [JsonPropertyName("yieldtotal")] public double? YieldTotal { get; set; }
        [JsonPropertyName("feedinpower")] public double? FeedInPower { get; set; }
        [JsonPropertyName("feedinenergy")] public double? FeedInEnergy { get; set; }
        [JsonPropertyName("consumeenergy")] public double? ConsumeEnergy { get; set; }
        [JsonPropertyName("feedinpowerM2")] public double? FeedInPowerM2 { get; set; }
        [JsonPropertyName("soc")] public double? Soc { get; set; }
        [JsonPropertyName("peps1")] public double? Peps1 { get; set; }
        [JsonPropertyName("peps2")] public double? Peps2 { get; set; }
        [JsonPropertyName("peps3")] public double? Peps3 { get; set; }
        [JsonPropertyName("inverterType")] public string? InverterType { get; set; }
        [JsonPropertyName("inverterStatus")] public string? InverterStatus { get; set; }
        [JsonPropertyName("uploadTime")] public string? UploadTime { get; set; }
        [JsonPropertyName("batPower")] public double? BatPower { get; set; }
        [JsonPropertyName("powerdc1")] public double? PowerDc1 { get; set; }
        [JsonPropertyName("powerdc2")] public double? PowerDc2 { get; set; }
        [JsonPropertyName("powerdc3")] public double? PowerDc3 { get; set; }
        [JsonPropertyName("powerdc4")] public double? PowerDc4 { get; set; }
        [JsonPropertyName("batStatus")] public string? BatStatus { get; set; }
    }
}
