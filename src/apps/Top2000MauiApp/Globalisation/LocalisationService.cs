using System.Globalization;

namespace Top2000MauiApp.Globalisation;

public class LocalisationService : ILocalisationService
{
    public const string CulturePreferenceName = "Culture";
    private readonly IEnumerable<ICulture> _cultures;
    private ICulture _activeCulture;

    public LocalisationService(IEnumerable<ICulture> cultures)
    {
        _cultures = cultures;
        _activeCulture = cultures.Single(x => x.Name == "nl");
    }

    public ICulture GetCurrentCulture() => _activeCulture;

    public void SetCulture(ICulture cultureInfo)
    {
        if (cultureInfo != this.GetCurrentCulture())
        {
            Preferences.Set(CulturePreferenceName, cultureInfo.Name);

            _activeCulture = cultureInfo;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo.Name);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo.Name);

            Translator.Instance.Invalidate();
        }
    }

    public void SetCultureFromSetting()
    {
        var name = Preferences.Get(CulturePreferenceName, "nl");
        _activeCulture = this.FindCulture(name);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(name);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);

        Translator.Instance.Invalidate();
    }

    private ICulture FindCulture(string name)
        => _cultures.SingleOrDefault(x => x.Name == name) ?? _cultures.Single(x => x.Name == "nl");
}
