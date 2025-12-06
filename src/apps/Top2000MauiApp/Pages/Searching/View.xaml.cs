using Top2000.Features.Searching;
using Top2000MauiApp.Globalisation;

namespace Top2000MauiApp.Pages.Searching;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage, ISortGroupNameProvider
{
    public View()
    {
        this.BindingContext = App.GetService<ViewModel>();
        this.InitializeComponent();
    }

    public ViewModel ViewModel => (ViewModel)this.BindingContext;

    public string GetNameForGroup(IGroup group)
    {
        return GetNameForGroupOrSortBy(group)
            ?? throw new NotImplementedException($"Group '{group.GetType()}' was not defined yet.");
    }

    public string GetNameForSort(ISort sort)
    {
        return GetNameForGroupOrSortBy(sort)
            ?? throw new NotImplementedException($"Display text for sort option '{sort.GetType()}' was not defined.");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.ViewModel.OnActivate(this);

        if (string.IsNullOrWhiteSpace(this.ViewModel.QueryText))
        {
            if (this.ViewModel.GroupBy is not null)
            {
                this.ViewModel.IsFlat = this.ViewModel.GroupBy.Value is GroupByNothing;
            }
        }

        GroupSortLayout.IsVisible = false;
        this.SetTitlesForButton();
    }

    protected override bool OnBackButtonPressed()
    {
        if (trackInformation.IsVisible)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.CloseTrackInformationAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return true;
        }

        return base.OnBackButtonPressed();
    }

    private static string? GetNameForGroupOrSortBy(object item)
    {
        return item switch
        {
            GroupByNothing _ => AppResources.None,
            SortByArtist _ => AppResources.Artist,
            GroupByArtist _ => AppResources.Artist,
            SortByRecordedYear _ => AppResources.Year,
            GroupByRecordedYear _ => AppResources.Year,
            SortByTitle _ => AppResources.Title,
            _ => null,
        };
    }

    private async void OnListingSelected(object sender, SelectionChangedEventArgs e)
    {
        if (this.ViewModel.SelectedTrack is null || trackInformation.IsVisible)
        {
            return;
        }

        var infoTask = trackInformation.LoadTrackDetailsAsync(this.ViewModel.SelectedTrack.Id, this.CloseTrackInformationAsync);

        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        await trackInformation.TranslateTo(this.Width * -1, 0, 0);

        trackInformation.IsVisible = true;
        await trackInformation.TranslateTo(0, 0);

        await infoTask;
    }

    private async Task CloseTrackInformationAsync()
    {
        this.ViewModel.SelectedTrack = null;

        Shell.SetTabBarIsVisible(this, true);
        Shell.SetNavBarIsVisible(this, true);
        await trackInformation.TranslateTo(this.Width * -1, 0);
        trackInformation.IsVisible = false;
    }

    private async void OnGroupByButtonClick(object sender, System.EventArgs e)
    {
        var groups = this.ViewModel.GroupByOptions.Select(x => x.Name).ToArray();

        var toGroup = await this.DisplayActionSheet(AppResources.GroupByHeader, AppResources.Cancel, null, groups);

        if (!string.IsNullOrEmpty(toGroup) && toGroup != AppResources.Cancel)
        {
            GroupByButton.Text = $"{Translator.Instance["GroupByHeader"]} {toGroup}";

            var groupBy = this.ViewModel.GroupByOptions.SingleOrDefault(x => x.Name == toGroup);
            if (groupBy != null)
            {
                this.ViewModel.GroupBy = groupBy;
                this.ViewModel.ReSortGroup();
            }
        }
    }

    private async void OnSortByButtonClick(object sender, System.EventArgs e)
    {
        var sortings = this.ViewModel.SortByOptions.Select(x => x.Name).ToArray();

        var toSort = await this.DisplayActionSheet(AppResources.SortByHeader, AppResources.Cancel, null, sortings);

        if (!string.IsNullOrEmpty(toSort) && toSort != AppResources.Cancel)
        {
            SortByButton.Text = $"{Translator.Instance["SortByHeader"]} {toSort}";

            var sortBy = this.ViewModel.SortByOptions.SingleOrDefault(x => x.Name == toSort);
            if (sortBy != null)
            {
                this.ViewModel.SortBy = sortBy;
                this.ViewModel.ReSortGroup();
            }
        }
    }

    private void SetTitlesForButton()
    {
        GroupByButton.Text = $"{Translator.Instance["GroupByHeader"]} {this.ViewModel.GroupBy?.Name}";
        SortByButton.Text = $"{Translator.Instance["SortByHeader"]} {this.ViewModel.SortBy?.Name}";
    }

    private void ShowSortGroupLayout(object sender, EventArgs e)
    {
        GroupSortLayout.IsVisible = !GroupSortLayout.IsVisible;
    }
}
