using CommunityToolkit.Mvvm.ComponentModel;
using HomeAutomation.Application.BatteryData;
using HomeAutomation.Application.WeatherForecast;
using HomeAutomation.Application.InverterSettings;
using HomeAutomation.Application.EnergyPricing;
using HomeAutomation.Domain.Weather;
using HomeAutomation.Domain.Inverter;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace HomeAutomation.Desktop.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IMediator? _mediator;
    private DispatcherTimer? _refreshTimer;
    private CancellationTokenSource? _refreshCancellation;

    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private BatteryViewModel? battery;

    [ObservableProperty]
    private WeatherViewModel? weather;

    [ObservableProperty]
    private InverterViewModel? inverter;

    [ObservableProperty]
    private EnergyPricingViewModel? energyPricing;

    [ObservableProperty]
    private DateTime? lastUpdated;

    public DashboardViewModel()
    {
        // Designer support
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        // For design-time data
        Battery = new BatteryViewModel
        {
            PercentageCharged = 65,
            StateDescription = "Idle",
            ActivityDescription = "Idle"
        };
        Weather = new WeatherViewModel { IsSunny = true };
        Inverter = new InverterViewModel { CurrentSettingName = "Self Use" };
        EnergyPricing = new EnergyPricingViewModel { CurrentUnitRatePence = 14.49m };
        IsLoading = false;
    }

    public DashboardViewModel(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        InitializeRefreshTimer();
        _ = RefreshAsync();
    }

    private void InitializeRefreshTimer()
    {
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(60)
        };
        _refreshTimer.Tick += async (_, _) => await RefreshAsync();
        _refreshTimer.Start();
    }

    public async Task RefreshAsync()
    {
        if (_mediator == null) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            _refreshCancellation?.Cancel();
            _refreshCancellation = new CancellationTokenSource();

            // Fetch all data in parallel
            var batteryTask = _mediator.Send(new GetBatteryData(), _refreshCancellation.Token);
            var weatherTask = _mediator.Send(new GetWeatherForecast(), _refreshCancellation.Token);
            var inverterTask = _mediator.Send(new GetInverterSettings(), _refreshCancellation.Token);
            var pricingTask = _mediator.Send(new GetEnergyPricing(), _refreshCancellation.Token);

            await Task.WhenAll(batteryTask, weatherTask, inverterTask, pricingTask);

            // Update ViewModels with fetched data
            var batteryData = await batteryTask;
            Battery = new BatteryViewModel(batteryData);

            var weatherData = await weatherTask;
            if (weatherData != null)
            {
                Weather = new WeatherViewModel(weatherData);
            }

            var inverterData = await inverterTask;
            if (inverterData != null)
            {
                Inverter = new InverterViewModel(inverterData);
            }

            var pricingData = await pricingTask;
            EnergyPricing = new EnergyPricingViewModel(pricingData);

            LastUpdated = DateTime.Now;
            IsLoading = false;
        }
        catch (OperationCanceledException)
        {
            // Refresh was cancelled, ignore
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error refreshing data: {ex.Message}";
            IsLoading = false;
        }
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
        _refreshCancellation?.Cancel();
        _refreshCancellation?.Dispose();
    }
}
