using System.Text.Json.Serialization;

namespace HomeAutomation.MetOffice;

internal class OpenMeteoResponse
{
    [JsonPropertyName("current_weather")]
    public CurrentWeather CurrentWeather { get; set; } = new();
}

internal class CurrentWeather
{
    [JsonPropertyName("weathercode")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;
}
