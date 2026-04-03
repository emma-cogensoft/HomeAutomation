namespace HomeAutomation.CloudInverter;

public class CloudInverterException : Exception
{
    public CloudInverterException(string message) : base(message) { }
    public CloudInverterException(string message, Exception innerException) : base(message, innerException) { }
}