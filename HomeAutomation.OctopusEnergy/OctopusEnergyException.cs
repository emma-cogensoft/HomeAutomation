namespace HomeAutomation.OctopusEnergy;

public class OctopusEnergyException : Exception
{
    public OctopusEnergyException(string message)
        : base(message)
    {
    }

    public OctopusEnergyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
