using CommunityToolkit.Mvvm.ComponentModel;
using HomeAutomation.Domain.Inverter;
using System;

namespace HomeAutomation.Desktop.ViewModels;

public partial class InverterViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? currentSettingName;

    [ObservableProperty]
    private DateTime lastUpdated;

    [ObservableProperty]
    private bool isChargeFromGridEnabled;

    [ObservableProperty]
    private int minimumBatteryPercentage;

    [ObservableProperty]
    private int maximumBatteryPercentage;

    public InverterViewModel()
    {
        // Design-time data
        CurrentSettingName = "Self Use";
        LastUpdated = DateTime.Now;
        IsChargeFromGridEnabled = false;
        MinimumBatteryPercentage = 20;
        MaximumBatteryPercentage = 95;
    }

    public InverterViewModel(CurrentInverterSettings settings)
    {
        CurrentSettingName = settings.CurrentWorkTypeName;
        LastUpdated = settings.TimeStamp;

        if (settings.SelfUseSettings != null)
        {
            IsChargeFromGridEnabled = settings.SelfUseSettings.IsChargeFromGridEnabled;
            MinimumBatteryPercentage = settings.SelfUseSettings.MinimumAllowedBatteryPercentage;
            MaximumBatteryPercentage = settings.SelfUseSettings.MaximumBatteryPercentage;
        }
    }
}
