using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HomeAutomation.LocalInverter.InverterApiRealTimeData;

internal class LocalInverterApiResponse
{
    [JsonPropertyName("Data")] public JsonArray Data { get; set; } = new();
}