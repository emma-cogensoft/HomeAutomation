using CommunityToolkit.Mvvm.ComponentModel;
using HomeAutomation.Domain.Weather;

namespace HomeAutomation.Desktop.ViewModels;

public partial class WeatherViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isSunny;

    [ObservableProperty]
    private string? weatherDescription;

    public string WeatherBadge
    {
        get => IsSunny ? "☀ Sunny" : "☁ Cloudy";
    }

    public string WeatherClass
    {
        get => IsSunny ? "weather-sunny" : "weather-cloudy";
    }

    public WeatherViewModel()
    {
        // Design-time data
        IsSunny = true;
        WeatherDescription = "Sunny Day";
    }

    public WeatherViewModel(WeatherForecast weather)
    {
        IsSunny = weather.IsSunny;
        WeatherDescription = weather.WeatherType.ToString();
    }
}
