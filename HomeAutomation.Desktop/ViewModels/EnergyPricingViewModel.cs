using CommunityToolkit.Mvvm.ComponentModel;
using HomeAutomation.Application.EnergyPricing;
using System;
using System.Collections.ObjectModel;

namespace HomeAutomation.Desktop.ViewModels;

public partial class EnergyPricingViewModel : ViewModelBase
{
    [ObservableProperty]
    private decimal? currentUnitRatePence;

    [ObservableProperty]
    private string? currentPeriodStart;

    [ObservableProperty]
    private string? currentPeriodEnd;

    [ObservableProperty]
    private string? nextCheapestPeriodStart;

    [ObservableProperty]
    private string? nextCheapestPeriodEnd;

    [ObservableProperty]
    private ObservableCollection<EnergyPriceDetail> prices24H = new();

    public string PriceStatusBadge
    {
        get
        {
            if (CurrentUnitRatePence == null) return "N/A";
            return CurrentUnitRatePence switch
            {
                <= 15 => "💰 Cheap",
                <= 20 => "⚡ Medium",
                _ => "📈 Expensive"
            };
        }
    }

    public string PriceStatusClass
    {
        get
        {
            if (CurrentUnitRatePence == null) return "price-unknown";
            return CurrentUnitRatePence switch
            {
                <= 15 => "price-cheap",
                <= 20 => "price-medium",
                _ => "price-expensive"
            };
        }
    }

    public EnergyPricingViewModel()
    {
        // Design-time data
        CurrentUnitRatePence = 14.49m;
        CurrentPeriodStart = DateTime.Now.ToString("HH:mm");
        CurrentPeriodEnd = DateTime.Now.AddHours(1).ToString("HH:mm");
        NextCheapestPeriodStart = "02:00";
        NextCheapestPeriodEnd = "03:00";
        Prices24H = new ObservableCollection<EnergyPriceDetail>();
    }

    public EnergyPricingViewModel(EnergyPricingResponse? pricing)
    {
        if (pricing == null)
        {
            return;
        }

        CurrentUnitRatePence = pricing.CurrentUnitRatePence;

        if (pricing.CurrentPeriod != null)
        {
            CurrentPeriodStart = pricing.CurrentPeriod.ValidFrom.ToString("HH:mm");
            CurrentPeriodEnd = pricing.CurrentPeriod.ValidTo.ToString("HH:mm");
        }

        if (pricing.NextCheapestPeriod != null)
        {
            NextCheapestPeriodStart = pricing.NextCheapestPeriod.ValidFrom.ToString("HH:mm");
            NextCheapestPeriodEnd = pricing.NextCheapestPeriod.ValidTo.ToString("HH:mm");
        }

        Prices24H = new ObservableCollection<EnergyPriceDetail>(pricing.Prices24H);
    }
}
