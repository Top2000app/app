#if DEBUG
using Microsoft.Extensions.Logging;
#endif

using Top2000.Features.SQLite;
using Top2000MauiApp.Globalisation;
using Top2000MauiApp.Pages.NavigationShell;
using Top2000MauiApp.Themes;

namespace Top2000MauiApp;

public static partial class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("MaterialIcons.ttf", "MaterialIcons");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ILocalisationService, LocalisationService>();
        // services.AddTransient<IStoreReview, StoreReviewImplementation>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services
            .AddTop2000Features(configure =>
            {
                configure.DatabaseDirectory(FileSystem.Current.AppDataDirectory);
                configure.EnableOnlineUpdates();
            })
            .AddSingleton<IThemeService, ThemeService>()
            .AddTransient<Pages.Overview.Position.ViewModel>()
            .AddTransient<Pages.Overview.Date.ViewModel>()
            .AddTransient<Pages.TrackInformation.ViewModel>()
            .AddTransient<Pages.Searching.ViewModel>()
            // .AddTransient<IAskForReview, ReviewModule>()
            // .AddSingleton<Xamarin.Essentials.Interfaces.IPreferences, Xamarin.Essentials.Implementation.PreferencesImplementation>()
            .AddSingleton<ICulture>(new SupportedCulture("nl"))
            .AddSingleton<ICulture>(new SupportedCulture("en"))
            .AddSingleton<ICulture>(new SupportedCulture("fr"))
        ;

        if (Top2000Info.IsLive())
        {
            builder.Services.AddSingleton<IMainShell, Top2000MauiApp.Pages.NavigationShell.LiveTop2000.View>();
        }
        else
        {
            builder.Services.AddSingleton<IMainShell, Top2000MauiApp.Pages.NavigationShell.View>();
        }

        // builder.Services.Configure<AskForReviewConfiguration>(builder.Configuration.GetSection(nameof(AskForReviewConfiguration)));

        var serviceProvider = builder.Build();

        App.ServiceProvider = serviceProvider.Services;
        App.EnsureDatabaseIsCreatedAsync().GetAwaiter().GetResult();

        // Kick off online update check, but don't wait for it.
        App.CheckForOnlineUpdates();

        return serviceProvider;
    }
}
