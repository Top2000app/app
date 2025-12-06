using System.Globalization;

namespace Top2000MauiApp.Globalisation;

public class LocalisationService : ILocalisationService
{
    public const string CulturePreferenceName = "Culture";
    private readonly IEnumerable<ICulture> cultures;
    private ICulture activeCulture;

    public LocalisationService(IEnumerable<ICulture> cultures)
    {
        this.cultures = cultures;
        activeCulture = cultures.Single(x => x.Name == "nl");
    }

    public ICulture GetCurrentCulture() => activeCulture;

    public void SetCulture(ICulture cultureInfo)
    {
        if (cultureInfo != this.GetCurrentCulture())
        {
            Preferences.Set(CulturePreferenceName, cultureInfo.Name);

            activeCulture = cultureInfo;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo.Name);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo.Name);

            Translator.Instance.Invalidate();
        }
    }

    public void SetCultureFromSetting()
    {
        var name = Preferences.Get(CulturePreferenceName, "nl");
        activeCulture = this.FindCulture(name);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(name);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);

        Translator.Instance.Invalidate();
    }

    private ICulture FindCulture(string name)
        => cultures.SingleOrDefault(x => x.Name == name) ?? cultures.Single(x => x.Name == "nl");
}
