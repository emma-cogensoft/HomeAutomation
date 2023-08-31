namespace HomeAutomation.Domain.Weather;

public class WeatherForecast
{
    public WeatherType WeatherType { get; init; }
    public bool IsSunny => WeatherType is WeatherType.SunnyDay or WeatherType.PartlyCloudyDay;
    
    public WeatherForecast(WeatherType weatherType)
    {
        WeatherType = weatherType;
    }
}