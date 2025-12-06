using Top2000MauiApp.Globalisation;
using Top2000MauiApp.Pages.NavigationShell;
using Top2000MauiApp.Themes;

namespace Top2000MauiApp.Pages.Settings;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class General : ContentPage
{
    private readonly ILocalisationService localisationService;
    private readonly IThemeService themeService;
    private readonly IEnumerable<ICulture> cultures;

    public General()
    {
        InitializeComponent();

        localisationService = App.GetService<ILocalisationService>();
        themeService = App.GetService<IThemeService>();
        cultures = App.GetService<IEnumerable<ICulture>>();
    }

    protected override void OnAppearing()
    {
        var currentCulture = localisationService.GetCurrentCulture();
        switch (currentCulture.Name)
        {
            case "nl":
                SetRadioButtons(this.nl);
                break;

            case "en":
                SetRadioButtons(this.en);
                break;

            case "fr":
                SetRadioButtons(this.fr);
                break;

            default:
                break;
        }

        var currentTheme = themeService.CurrentThemeName;
        if (currentTheme == Dark.ThemeName)
        {
            useDarkModeSwitch.IsToggled = true;
        }
    }

    private void OnUseDarkModeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            themeService.SetTheme(Dark.ThemeName);
        }
        else
        {
            themeService.SetTheme(Light.ThemeName);
        }
    }

    private void ShowInDutch(object sender, System.EventArgs e)
    {
        SetRadioButtons(this.nl);
        SetCulture("nl");
    }

    private void ShowInEnglish(object sender, System.EventArgs e)
    {
        SetRadioButtons(this.en);
        SetCulture("en");
    }

    private void ShowInFrench(object sender, System.EventArgs e)
    {
        SetRadioButtons(this.fr);
        SetCulture("fr");
    }

    private void SetRadioButtons(Label active)
    {
        this.en.Text = Symbols.RadioButtonOpen;
        this.nl.Text = Symbols.RadioButtonOpen;
        this.fr.Text = Symbols.RadioButtonOpen;

        active.Text = Symbols.RadioButtonChecked;
    }

    private void SetCulture(string name)
    {
        localisationService.SetCulture(cultures.Single(x => x.Name == name));
        App.GetService<IMainShell>().SetTitles();
    }
}
