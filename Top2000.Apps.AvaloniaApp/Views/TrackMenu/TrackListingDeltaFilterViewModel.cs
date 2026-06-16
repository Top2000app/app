using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public partial class TrackListingDeltaFilterViewModel : ObservableObject
{
    private static readonly SolidColorBrush WhiteColourBrush = new (Colors.White);

    public required TrackListingDeltaType FilterBy { get; init; }
    
    public required string DisplayIcon { get; init; }

    public required SolidColorBrush DisplayColour { get; init; }

    public required ICanFilterListings Parent { get; init; }

    [ObservableProperty]
    public partial int Count { get; set; } = 0;
    
    [ObservableProperty]
    public partial bool IsChecked { get; set; }

    [ObservableProperty]
    public partial SolidColorBrush DisplayColourBrush { get; set; } = new(Colors.White);

    partial void OnIsCheckedChanged(bool value)
    {
        Parent.UpdateListings();

        DisplayColourBrush = value
            ? WhiteColourBrush
            : DisplayColour;
    }
}