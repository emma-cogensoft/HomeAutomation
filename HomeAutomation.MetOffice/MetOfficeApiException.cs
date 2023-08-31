namespace HomeAutomation.MetOffice;

public class MetOfficeApiException : Exception
{
    public MetOfficeApiException(string message): base(message) { }
}