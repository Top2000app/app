namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class SearchTrackResultViewModel
{
    public required int TrackId { get; init; }
    public required string TitleWithRecordedYear { get; init; }
    public required string Artist { get; init; }
    public required string PositionInLatestEdition { get; init; }
}