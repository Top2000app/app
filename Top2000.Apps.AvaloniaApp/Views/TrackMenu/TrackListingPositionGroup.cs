namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class TrackListingPositionGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPositionGroup);
    public required string GroupName { get; init; }
    
    public required int PositionRangStart { get; init; }
    public required int PositionRangEnd { get; init; }
}