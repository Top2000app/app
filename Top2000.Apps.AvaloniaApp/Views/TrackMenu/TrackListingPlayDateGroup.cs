namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class TrackListingPlayDateGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPlayDateGroup);
    public required string GroupName { get; init; }
    
    public required List<TrackListingPlayTimeItem> PlayTimes { get; init; } 
}