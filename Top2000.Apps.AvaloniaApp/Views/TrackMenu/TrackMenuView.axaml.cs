using Avalonia.Controls;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public partial class TrackMenuView : UserControl, IScrollListingListIntoView
{
    public TrackMenuView()
    {
        InitializeComponent();
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