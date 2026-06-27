using Avalonia.Media;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class TrackListingViewModel : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingViewModel);
    
    public required int TrackId { get; init; }
    public required string PositionString { get; init; }
    public required string Delta { get; init; }
    public required string DeltaSymbol { get; init; }
    public required Brush DeltaSymbolColour { get; init; }
    public required double DeltaFontSize { get; init; }

    public required string Title { get; init; }
    public required string Artist { get; init; }

    public required int Position { get; init; }

    public required DateTime LocalPlayDateTime { get; init; }
    
    
    public static string ConvertDeltaToString(TrackListing track)
    {
        return track.Delta != 0
            ? Math.Abs(track.Delta).ToString()
            : string.Empty;
    }
}