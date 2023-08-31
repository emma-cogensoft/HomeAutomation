using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HomeAutomation.LocalInverter;

internal class InverterLocalApiResponse
{
    [JsonPropertyName("Data")] public JsonArray Data { get; set; } = new();
}