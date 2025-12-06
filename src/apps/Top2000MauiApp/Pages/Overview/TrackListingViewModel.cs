using Top2000.Features.AllListingsOfEdition;
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
        var value = track.Delta;

        if (value == 0)
        {
            return 15;
        }

        if (value is null || !value.HasValue)
        {
            return 20;
        }

        return 11;
    }

    public static string ConvertDeltaToSymbol(TrackListing track)
    {
        var value = track.Delta;

        if (value is null)
        {
            if (track.IsRecurring)
            {
                return Symbols.BackInList;
            }

            return Symbols.New;
        }

        if (value.Value > 0)
        {
            return Symbols.Up;
        }

        if (value.Value < 0)
        {
            return Symbols.Down;
        }

        return Symbols.Same;
    }

    public static string ConvertDeltaToString(TrackListing track)
    {
        var offset = track.Delta;
        return offset.HasValue && offset.Value != 0
            ? Math.Abs(offset.Value).ToString(App.NumberFormatProvider)
            : string.Empty;
    }

    public static Color ConvertDeltaSymbolColour(TrackListing track)
    {
        var value = track.Delta;

        if (!value.HasValue)
        {
            return Colours.YellowColour;
        }

        if (value.Value > 0)
        {
            return Colours.GreenColour;
        }

        if (value.Value < 0)
        {
            return Colours.RedColour;
        }

        return Colours.GreyColour;
    }
}
