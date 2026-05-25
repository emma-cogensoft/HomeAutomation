using Avalonia;
using HomeAutomation.Application;
using HomeAutomation.CloudInverter;
using HomeAutomation.LocalInverter;
using HomeAutomation.MetOffice;
using HomeAutomation.OctopusEnergy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HomeAutomation.Desktop;

sealed class Program
{
    public static IServiceProvider? Services { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        Services = ConfigureServices();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    private static IServiceProvider ConfigureServices()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var services = new ServiceCollection();

        // Register configuration options
        services
            .AddOptions<HomeAutomation.LocalInverter.LocalInverterApiOptions>()
            .Bind(configuration.GetSection("Services:LocalInverterApiSettingsOptions"))
            .ValidateDataAnnotations();

        services
            .AddOptions<HomeAutomation.CloudInverter.CloudInverterApiOptions>()
            .Bind(configuration.GetSection("Services:CloudInverterApiSettingsOptions"))
            .ValidateDataAnnotations();

        services
            .AddOptions<HomeAutomation.MetOffice.OpenMeteoApiOptions>()
            .Bind(configuration.GetSection("Services:OpenMeteoApiSettingsOptions"))
            .ValidateDataAnnotations();

        services
            .AddOptions<HomeAutomation.Application.BatteryData.BatteryOptions>()
            .Bind(configuration.GetSection("Services:BatteryOptions"));

        services
            .AddOptions<HomeAutomation.OctopusEnergy.OctopusEnergyApiOptions>()
            .Bind(configuration.GetSection("Services:OctopusEnergyApiOptions"))
            .ValidateDataAnnotations();

        // Register application services
        services.RegisterApplicationServices();
        services.RegisterCloudInverterServices();
        services.RegisterLocalInverterServices();
        services.RegisterWeatherServices();
        services.RegisterOctopusEnergyServices();

        return services.BuildServiceProvider();
    }
}
