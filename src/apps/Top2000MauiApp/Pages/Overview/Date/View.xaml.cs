using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using static Top2000MauiApp.MauiProgram;

namespace Top2000MauiApp.Pages.Overview.Date;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private static bool isFirstScreenAppearing = true;
    public View()
    {
        this.BindingContext = App.GetService<ViewModel>();
        this.InitializeComponent();
    }

    public ViewModel ViewModel => (ViewModel)this.BindingContext;

    protected override bool OnBackButtonPressed()
    {
        if (GroupFlyout.IsVisible)
        {
            Shell.SetTabBarIsVisible(this, true);
            Shell.SetNavBarIsVisible(this, true);
            GroupFlyout.TranslateToAsync(this.Width * -1, 0);
            GroupFlyout.IsVisible = false;

            return true;
        }

        if (trackInformation.IsVisible)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.CloseTrackInformationAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return true;
        }

        return base.OnBackButtonPressed();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (this.ViewModel.Listings.Count == 0)
        {
            await this.ViewModel.InitialiseViewModelAsync();
        }

        if (isFirstScreenAppearing)
        {
            isFirstScreenAppearing = false;
            await Task.Delay(100);
            this.JumpWhenTop2000IsOn();
        }
        else
        {
            this.JumpWhenTop2000IsOn();
        }
    }
    
    private void JumpWhenTop2000IsOn()
    {
        if (Top2000Info.ShowLiveScreen())
        {
            this.JumpToSelectedDateTime(DateTime.Now);
        }
        else
        {
            this.ToolbarItems.Remove(jumpToToday);
        }
    }

    private async void OnJumpGroupButtonClick(object sender, EventArgs e)
    {
        if (trackInformation.IsVisible) { return; }

        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        await GroupFlyout.TranslateToAsync(this.Width * -1, 0, 0);

        GroupFlyout.IsVisible = true;
        await GroupFlyout.TranslateToAsync(0, 0);
    }

    private async void OnGroupSelected(object sender, SelectionChangedEventArgs e)
    {
        if (dates.SelectedItem is DateTime date)
        {
            Shell.SetTabBarIsVisible(this, true);
            Shell.SetNavBarIsVisible(this, true);
            await GroupFlyout.TranslateToAsync(this.Width * -1, 0);
            GroupFlyout.IsVisible = false;

            this.JumpToSelectedDateTime(date);
            dates.SelectedItem = null;
        }
    }

    private void JumpToSelectedDateTime(DateTime selectedDate)
    {
        if (trackInformation.IsVisible) { return; }

        var group = this.ViewModel
            .Listings
            .LastOrDefault(x => x.Key <= selectedDate);

        if (group is not null)
        {
            var firstGroup = group.FirstOrDefault();
            if (firstGroup is not null)
            {
                this.listings.ScrollTo(group.First(), position: ScrollToPosition.Center, animate: false);
            }
        }
    }

    private void OpenTodayClick(object sender, EventArgs e)
    {
        this.JumpToSelectedDateTime(DateTime.Now);
    }

    private async void OnListingSelected(object sender, SelectionChangedEventArgs e)
    {
        if (this.ViewModel.SelectedListing is null || trackInformation.IsVisible)
        {
            return;
        }

        var infoTask = trackInformation.LoadTrackDetailsAsync(this.ViewModel.SelectedListing.TrackId, this.CloseTrackInformationAsync);

        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        await trackInformation.TranslateToAsync(this.Width * -1, 0, 0);

        trackInformation.IsVisible = true;
        await trackInformation.TranslateToAsync(0, 0);

        await infoTask;
    }

    private async Task CloseTrackInformationAsync()
    {
        this.ViewModel.SelectedListing = null;

        Shell.SetTabBarIsVisible(this, true);
        Shell.SetNavBarIsVisible(this, true);
        await trackInformation.TranslateToAsync(this.Width * -1, 0);
        trackInformation.IsVisible = false;
    }
}
