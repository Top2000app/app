using Top2000MauiApp.AskForReview;
using Top2000MauiApp.Globalisation;
namespace Top2000MauiApp.Pages.NavigationShell;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : Shell, IMainShell
{
    public View()
    {
        InitializeComponent();

        //#pragma warning disable 4014
        //            Task.Run(async () =>
        //            {
        //                await Task.Delay(5 * 1000);
        //                CheckForReviewAsync();
        //            }).ConfigureAwait(false);
        //#pragma warning restore 4014
    }

    public bool IsViewForWhenTop2000IsLive => false;

    public void SetTitles()
    {
        var strings = Translator.Instance;

        OverviewTab.Title = strings["Overview"];
        ViewByDateTab.Title = strings["ViewByDate"];
        SearchTab.Title = strings["Search"];
        SettingsTab.Title = strings["Settings"];

        GeneralTab.Title = strings["General"];
        AboutTab.Title = strings["About"];

        AboutTab.IsVisible = false;

        AboutTab.IsVisible = true;
    }

    public void CheckForReviewAsync()
    {
        var reviews = App.GetService<IAskForReview>();

        if (reviews.MustAskForReview())
        {
            var strings = Translator.Instance;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var rateIt = await DisplayAlert(strings["RateTitle"], strings["RateText"], strings["RateIt"], strings["RateNotNow"]);

                if (rateIt)
                {
                    reviews.AskForReview();
                }
            });
        }
    }
}
