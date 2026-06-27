using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public partial class TrackListingPosition : ObservableObject, ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPosition);
    public required string ItemText { get; init; }
    public required IHandleGroupSelection NewGroupHandler { get; init; }
    public required TrackListingPositionGroup Parent { get; init; }

    [RelayCommand]
    private void GotoPosition()
    {
        NewGroupHandler.SelectingGroup(Parent);
    }
}