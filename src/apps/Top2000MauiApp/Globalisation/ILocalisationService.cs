namespace Top2000MauiApp.Globalisation;

public interface ILocalisationService
{
    void SetCulture(ICulture cultureInfo);

    void SetCultureFromSetting();

    ICulture GetCurrentCulture();
}
