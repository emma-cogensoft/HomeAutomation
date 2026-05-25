using CommunityToolkit.Mvvm.ComponentModel;
using HomeAutomation.Application.BatteryData;
using HomeAutomation.Application.WeatherForecast;
using HomeAutomation.Application.InverterSettings;
using HomeAutomation.Application.EnergyPricing;
using HomeAutomation.Domain.Weather;
using HomeAutomation.Domain.Inverter;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace HomeAutomation.Desktop.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IMediator? _mediator;
    private readonly ILogger<DashboardViewModel>? _logger;
    private DispatcherTimer? _refreshTimer;
    private CancellationTokenSource? _refreshCancellation;
    private int _refreshCount = 0;

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

    public DashboardViewModel(IMediator mediator, ILogger<DashboardViewModel> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger;
        _logger?.LogInformation("DashboardViewModel initialized");
        InitializeRefreshTimer();
        _ = RefreshAsync();
    }

    private void InitializeRefreshTimer()
    {
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(60)
        };
        _refreshTimer.Tick += async (_, _) =>
        {
            _logger?.LogDebug("Timer tick - triggering refresh");
            await RefreshAsync();
        };
        _refreshTimer.Start();
        _logger?.LogInformation("Refresh timer started (60 second interval)");
    }

    public async Task RefreshAsync()
    {
        if (_mediator == null)
        {
            _logger?.LogWarning("RefreshAsync called but mediator is null");
            return;
        }

        _refreshCount++;
        _logger?.LogInformation("Refresh #{RefreshCount} starting", _refreshCount);

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            _refreshCancellation?.Cancel();
            _refreshCancellation = new CancellationTokenSource();

            // Fetch all data in parallel
            _logger?.LogDebug("Fetching battery data...");
            var batteryTask = _mediator.Send(new GetBatteryData(), _refreshCancellation.Token);

            _logger?.LogDebug("Fetching weather data...");
            var weatherTask = _mediator.Send(new GetWeatherForecast(), _refreshCancellation.Token);

            _logger?.LogDebug("Fetching inverter settings...");
            var inverterTask = _mediator.Send(new GetInverterSettings(), _refreshCancellation.Token);

            _logger?.LogDebug("Fetching energy pricing...");
            var pricingTask = _mediator.Send(new GetEnergyPricing(), _refreshCancellation.Token);

            await Task.WhenAll(batteryTask, weatherTask, inverterTask, pricingTask);

            // Update ViewModels with fetched data
            var batteryData = await batteryTask;
            Battery = new BatteryViewModel(batteryData);
            _logger?.LogInformation("Battery data updated: {PercentageCharged}% from {DataSource}",
                Battery.PercentageCharged, Battery.DataSource);

            var weatherData = await weatherTask;
            if (weatherData != null)
            {
                Weather = new WeatherViewModel(weatherData);
                _logger?.LogInformation("Weather data updated: {IsSunny}", Weather.IsSunny ? "Sunny" : "Cloudy");
            }

            var inverterData = await inverterTask;
            if (inverterData != null)
            {
                Inverter = new InverterViewModel(inverterData);
                _logger?.LogInformation("Inverter data updated: {Mode}", Inverter.CurrentSettingName);
            }

            var pricingData = await pricingTask;
            EnergyPricing = new EnergyPricingViewModel(pricingData);
            _logger?.LogInformation("Energy pricing updated: {CurrentRate}p/kWh",
                EnergyPricing.CurrentUnitRatePence ?? 0);

            LastUpdated = DateTime.Now;
            IsLoading = false;
            _logger?.LogInformation("Refresh #{RefreshCount} completed successfully", _refreshCount);
        }
        catch (OperationCanceledException)
        {
            _logger?.LogDebug("Refresh #{RefreshCount} was cancelled", _refreshCount);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error refreshing data: {ex.Message}";
            IsLoading = false;
            _logger?.LogError(ex, "Refresh #{RefreshCount} failed: {ErrorMessage}", _refreshCount, ex.Message);
        }
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
        _refreshCancellation?.Cancel();
        _refreshCancellation?.Dispose();
    }
}
