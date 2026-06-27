namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class TrackListingPositionGroupHeader : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPositionGroupHeader);
    public required string  ItemText { get; init; }
    
    public required List<TrackListingPosition> PositionGroups { get; init; }
}