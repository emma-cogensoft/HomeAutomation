using CommunityToolkit.Mvvm.ComponentModel;
using HomeAutomation.Application.BatteryData;

namespace HomeAutomation.Desktop.ViewModels;

public partial class BatteryViewModel : ViewModelBase
{
    [ObservableProperty]
    private int percentageCharged;

    [ObservableProperty]
    private string? stateDescription;

    [ObservableProperty]
    private string? activityDescription;

    [ObservableProperty]
    private int remainingWh;

    [ObservableProperty]
    private int batteryPowerUsageInW;

    [ObservableProperty]
    private int timeToCompleteInH;

    [ObservableProperty]
    private int homeUsageInW;

    [ObservableProperty]
    private int solarInputInW;

    [ObservableProperty]
    private int feedInW;

    [ObservableProperty]
    private string? dataSource;

    public string StatusBadge
    {
        get
        {
            return PercentageCharged switch
            {
                >= 50 => "✓ Good",
                >= 20 => "⚠ Low",
                _ => "🔴 Critical"
            };
        }
    }

    public string StatusClass
    {
        get
        {
            return PercentageCharged switch
            {
                >= 50 => "status-good",
                >= 20 => "status-warning",
                _ => "status-critical"
            };
        }
    }

    public BatteryViewModel()
    {
        // Design-time data
        PercentageCharged = 65;
        StateDescription = "Charged";
        ActivityDescription = "Idle";
        RemainingWh = 7500;
        BatteryPowerUsageInW = 0;
        TimeToCompleteInH = 0;
        HomeUsageInW = 500;
        SolarInputInW = 2500;
        FeedInW = 2000;
        DataSource = "LocalInverter";
    }

    public BatteryViewModel(BatteryDataResult data)
    {
        PercentageCharged = data.BatteryInfo.BatteryState.PercentageCharged;
        StateDescription = data.BatteryInfo.BatteryState.Description;
        ActivityDescription = data.BatteryInfo.BatteryActivity.Description;
        RemainingWh = (int)data.BatteryInfo.BatteryState.RemainingBatteryCapacity;
        BatteryPowerUsageInW = (int)data.BatteryInfo.BatteryActivity.BatteryPowerUsage;
        TimeToCompleteInH = (int)data.BatteryInfo.BatteryActivity.TimeToComplete;
        HomeUsageInW = data.HomeUsageInW;
        SolarInputInW = data.SolarInputInW;
        FeedInW = data.FeedInW;
        DataSource = data.DataSource;
    }
}
