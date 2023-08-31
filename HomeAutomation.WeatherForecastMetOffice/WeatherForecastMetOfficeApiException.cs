namespace HomeAutomation.WeatherForecastMetOffice;

public class WeatherForecastMetOfficeApiException : Exception
{
    public WeatherForecastMetOfficeApiException(string message): base(message) { }
}