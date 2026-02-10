using Avalonia.Controls;
using Avalonia.Interactivity;
using Top2000.Apps.AvaloniaApp.ViewModels;

namespace Top2000.Apps.AvaloniaApp.Views;

public partial class MainWindow : Window, IScrollListingListIntoView
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ((MainWindowViewModel)DataContext!).InitialiseViewModelAsync(this);
    }

    public void ScrollIntoView(ITrackListingViewModel item)
    {
        var lastItem = ListingListBox.Items.Last();
        if (lastItem is not null)
        {
            ListingListBox.ScrollIntoView(lastItem);
        }
        
        ListingListBox.ScrollIntoView(item);
    }
}

public interface IScrollListingListIntoView
{
    void ScrollIntoView(ITrackListingViewModel item);
}