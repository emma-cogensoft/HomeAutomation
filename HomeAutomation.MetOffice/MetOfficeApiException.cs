namespace HomeAutomation.MetOffice;

public class MetOfficeApiException : Exception
{
    public MetOfficeApiException(string message) : base(message) { }
    public MetOfficeApiException(string message, Exception innerException) : base(message, innerException) { }
}