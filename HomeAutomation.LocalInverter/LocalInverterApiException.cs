namespace HomeAutomation.LocalInverter;

public class LocalInverterApiException : Exception
{
    public LocalInverterApiException(string message) : base(message) { }
    public LocalInverterApiException(string message, Exception innerException) : base(message, innerException) { }
}