namespace HomeAutomation.Application.Services.Inverter;

public class InverterRealtimeData
{
    public required double BatteryPercentage { get; init; } = -1;
    public required double BatteryPowerUsage { get; init; } = 0;
    public required double SolarInput { get; init; } = -1;
    public required double FeedIn { get; init; } = -1;
    public required double HomeUsage { get; init; } = -1;
    public required DateTime TimeStamp { get; init; }
    public required string Source { get; init; } = string.Empty;
}