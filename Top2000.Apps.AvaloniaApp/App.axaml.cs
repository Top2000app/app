using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Apps.AvaloniaApp.Views.Details;
using Top2000.Apps.AvaloniaApp.Views.Shell;
using Top2000.Apps.AvaloniaApp.Views.TrackMenu;
using Top2000.Features;
using Top2000.Features.SQLite;

namespace Top2000.Apps.AvaloniaApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Register all the services needed for the application to run
        var services = new ServiceCollection()
            .AddTransient<ShellViewModel>()
            .AddTransient<TrackMenuViewModel>()
            .AddTransient<DetailsViewModel>()
            .AddTop2000Features<SqliteFeatureAdapter>()
            .BuildServiceProvider();

        // Creates a ServiceProvider containing services from the provided IServiceCollection
        var vm = services.GetRequiredService<ShellViewModel>();
        var mainWindow = new ShellWindow() {  DataContext = vm };
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = mainWindow;
        }

        Dispatcher.UIThread.UnhandledException += OnUnhandledException;
        
        base.OnFrameworkInitializationCompleted();

        _ = vm.InitialiseAsync(
            trackMenuViewModel: services.GetRequiredService<TrackMenuViewModel>(),
            detailsViewModel: services.GetRequiredService<DetailsViewModel>()
            );
    }

    private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Log the exception
        Console.WriteLine("Unhandled UI thread exception: " + e.Exception);

        // Optionally prevent the application from crashing
        e.Handled = false;    
    }
}