using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public partial class TrackMenuView : UserControl, IScrollListingListIntoView
{
    public TrackMenuView()
    {
        InitializeComponent();

        SearchGrid.PropertyChanged += SearchGrid_OnPropertyChanged;
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

    private void SearchGrid_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.IsVisibleProperty && SearchGrid.IsVisible)
        {
            Dispatcher.UIThread.Post(() => SearchText.Focus(), DispatcherPriority.Input);
        }
    }

}


public interface IScrollListingListIntoView
{
    void ScrollIntoView(ITrackListingViewModel item);
}
