using Avalonia;
using HomeAutomation.Application;
using HomeAutomation.CloudInverter;
using HomeAutomation.Desktop.ViewModels;
using HomeAutomation.LocalInverter;
using HomeAutomation.MetOffice;
using HomeAutomation.OctopusEnergy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;

namespace HomeAutomation.Desktop;

sealed class Program
{
    public static IServiceProvider? Services { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            ConfigureLogging();
            Log.Information("=== HomeAutomation Desktop App Starting ===");
            Services = ConfigureServices();
            Log.Information("Services configured successfully");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.Information("=== HomeAutomation Desktop App Shutting Down ===");
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureLogging()
    {
        var logPath = System.IO.Path.Combine(AppContext.BaseDirectory, "logs", "app-.txt");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(logPath,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 10_000_000,
                retainedFileCountLimit: 10)
            .CreateLogger();
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

        if (OperatingSystem.IsWindows())
        {
            Log.Debug("Loading user secrets");
            builder.AddUserSecrets<Program>();
        }

        var configuration = builder.Build();
        Log.Debug("Configuration loaded. Services: {@Services}",
            configuration.GetSection("Services").AsEnumerable().Where(x => x.Value != null));

        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

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

        // Register ViewModels
        services.AddScoped<DashboardViewModel>();
        services.AddScoped<BatteryViewModel>();
        services.AddScoped<WeatherViewModel>();
        services.AddScoped<InverterViewModel>();
        services.AddScoped<EnergyPricingViewModel>();
        services.AddScoped<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
