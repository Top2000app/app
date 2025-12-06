namespace Top2000MauiApp.Pages.TrackInformation;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : Grid
{
    public View()
    {
        this.BindingContext = App.GetService<ViewModel>();
        this.InitializeComponent();
    }

    public ViewModel ViewModel => (ViewModel)this.BindingContext;

    private Func<Task>? OnClose { get; set; }

    public async Task LoadTrackDetailsAsync(int trackId, Func<Task> onClose)
    {
        this.OnClose = onClose;
        await positionsScroll.ScrollToAsync(0, 0, animated: false);

        await this.ViewModel.LoadTrackDetailsAsync(trackId);
    }

    private async void OnCloseButtonClick(object sender, System.EventArgs e)
    {
        if (this.OnClose != null)
        {
            await this.OnClose.Invoke();
        }
    }

    private async void OnViewVideoClick(object sender, EventArgs e)
    {
        var trackTitle = this.ViewModel?.Title?.Replace(' ', '+');
        var artistName = this.ViewModel?.Artist?.Replace(' ', '+');

        var url = new Uri($"https://duckduckgo.com/?q=!ducky+onsite:www.youtube.com+{trackTitle}+{artistName}");

        await Launcher.OpenAsync(url);
    }
}
