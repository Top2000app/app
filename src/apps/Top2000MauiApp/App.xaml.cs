using System.Globalization;
using Top2000.Data.ClientDatabase;
using Top2000MauiApp.Globalisation;
using Top2000MauiApp.Pages.NavigationShell;
using Top2000MauiApp.Themes;


namespace Top2000MauiApp;

public partial class App : Application
{
    public static readonly IFormatProvider DateTimeFormatProvider = DateTimeFormatInfo.InvariantInfo;

    public static readonly IFormatProvider NumberFormatProvider = NumberFormatInfo.InvariantInfo;

    private static IServiceProvider? serviceProvider;

    public App()
    {
        this.InitializeComponent();
        SetCulture();
        SetTheme();

        var navigationShell = GetService<IMainShell>();

        this.MainPage = navigationShell as Shell
            ?? throw new NotSupportedException($"{nameof(IMainShell)} must be implemented inside a Shell view");

        navigationShell.SetTitles();
    }

    public static IServiceProvider ServiceProvider
    {
        get
        {
            return serviceProvider ??
                throw new InvalidOperationException("Application isn't booted yet");
        }
        set
        {
            serviceProvider = value;
        }
    }

    public static T GetService<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();

    public static Task EnsureDatabaseIsCreatedAsync()
    {
        var databaseGen = GetService<IUpdateClientDatabase>();
        var top2000 = GetService<Top2000AssemblyDataSource>();

        return databaseGen.RunAsync(top2000);
    }

    public static async Task CheckForOnlineUpdates()
    {
        try
        {
            await Task.Delay(3_000);
            var databasGen = GetService<IUpdateClientDatabase>();
            var onlineStore = GetService<OnlineDataSource>();

            await databasGen.RunAsync(onlineStore);
        }
        catch
        {
            // I don't want a crash here, just continue. 
        }
    }

    private static void SetCulture()
    {
        var localisationService = App.GetService<ILocalisationService>();
        localisationService.SetCultureFromSetting();
    }

    private static void SetTheme()
    {
        var themeService = GetService<IThemeService>();
        themeService.SetThemeFromSetting();
    }
}
