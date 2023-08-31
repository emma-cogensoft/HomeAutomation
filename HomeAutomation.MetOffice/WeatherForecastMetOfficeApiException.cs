namespace HomeAutomation.MetOffice;

public class WeatherForecastMetOfficeApiException : Exception
{
    public WeatherForecastMetOfficeApiException(string message): base(message) { }
}