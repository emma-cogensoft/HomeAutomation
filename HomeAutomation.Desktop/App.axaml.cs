using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System;
using Avalonia.Markup.Xaml;
using HomeAutomation.Desktop.ViewModels;
using HomeAutomation.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HomeAutomation.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            Log.Information("OnFrameworkInitializationCompleted starting");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Log.Debug("Resolving DashboardViewModel from DI container");
                var viewModel = Program.Services?.GetService(typeof(DashboardViewModel)) as DashboardViewModel
                    ?? new DashboardViewModel();

                Log.Information("Creating MainWindow with DashboardViewModel");
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel,
                };
            }

            Log.Information("OnFrameworkInitializationCompleted completed");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error during framework initialization");
            throw;
        }

        base.OnFrameworkInitializationCompleted();
    }
}