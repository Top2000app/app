using Top2000.Features.Listings;
using Top2000MauiApp.Themes;

namespace Top2000MauiApp.Pages.Overview;

public class TrackListingViewModel
{
    public required int TrackId { get; init; }
    public required string PositionString { get; init; }
    public required string Delta { get; init; }
    public required string DeltaSymbol { get; init; }
    public required Color DeltaSymbolColour { get; init; }
    public required double DeltaFontSize { get; init; }

    public required string Title { get; init; }
    public required string Artist { get; init; }

    public required int Position { get; init; }

    public required DateTime LocalPlayDateTime { get; init; }

    public static double ConvertDeltaFontSize(TrackListing track)
    {
        return track.DeltaType switch
        {
            TrackListingDeltaType.NoChange => 15,
            TrackListingDeltaType.Increased => 11,
            TrackListingDeltaType.Decreased => 11,
            TrackListingDeltaType.New => 20,
            TrackListingDeltaType.Recurring => 20,
            _ => 11
        };
    }

    public static string ConvertDeltaToSymbol(TrackListing track)
    {
        return track.DeltaType switch
        {
            TrackListingDeltaType.NoChange => Symbols.Same,
            TrackListingDeltaType.Increased => Symbols.Up,
            TrackListingDeltaType.Decreased => Symbols.Down,
            TrackListingDeltaType.New => Symbols.New,
            TrackListingDeltaType.Recurring => Symbols.BackInList,
            _ => Symbols.Same
        };
    }

    public static string ConvertDeltaToString(TrackListing track)
    {
        return track.Delta != 0
            ? Math.Abs(track.Delta).ToString(App.NumberFormatProvider)
            : string.Empty;
    }

    public static Color ConvertDeltaSymbolColour(TrackListing track)
    {
        return track.DeltaType switch
        {
            TrackListingDeltaType.NoChange => Colours.GreyColour,
            TrackListingDeltaType.Increased => Colours.GreenColour,
            TrackListingDeltaType.Decreased => Colours.RedColour,
            TrackListingDeltaType.New => Colours.YellowColour,
            TrackListingDeltaType.Recurring => Colours.YellowColour,
            _ => Colours.GreyColour
        };
    }
}
