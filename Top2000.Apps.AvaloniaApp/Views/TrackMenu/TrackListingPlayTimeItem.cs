using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public partial class TrackListingPlayTimeItem : ObservableObject, ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPlayTimeItem);
    public required string Time { get; init; }
    public bool IsHeader => true;
    public required  ITrackListingViewModel Parent { get; init; }
    
    public required IHandleGroupSelection NewGroupHandler { get; init; }

    [RelayCommand]
    private void GotoPlayTime()
    {
        NewGroupHandler.SelectingGroup(Parent);
    }
}