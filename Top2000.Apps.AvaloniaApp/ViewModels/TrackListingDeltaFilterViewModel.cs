using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public partial class TrackListingDeltaFilterViewModel : ViewModelBase
{
    private static readonly SolidColorBrush WhiteColourBrush = new (Colors.White);

    public required TrackListingDeltaType FilterBy { get; init; }
    
    public required string DisplayIcon { get; init; }

    public required SolidColorBrush DisplayColour { get; init; }

    public required ICanFilterListings Parent { get; init; }

    [ObservableProperty] private int _count = 0;
    
    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty] 
    private SolidColorBrush _displayColourBrush = new (Colors.White);
    
    partial void OnIsCheckedChanged(bool value)
    {
        Parent.UpdateListings();

        DisplayColourBrush = value
            ? WhiteColourBrush
            : DisplayColour;
    }
}