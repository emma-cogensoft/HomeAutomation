namespace HomeAutomation.LocalInverter;

public class LocalInverterApiException : Exception
{
    public LocalInverterApiException(string message): base(message) { }
}