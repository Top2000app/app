using Avalonia.Media;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.Views.Details.TrackDetails;

public class TrackDetailsListingViewModel
{
    public required int Edition { get; init; }
    public string PositionString => Position?.ToString() ?? "-";
    public required int? Position { get; init; }
    public required string Delta { get; init; }
    public required string DeltaSymbol { get; init; }
    public required Brush DeltaSymbolColour { get; init; }
    public required double DeltaFontSize { get; init; }
    public required ListingStatus Status { get; init; }

    public static double ConvertDeltaFontSize(ListingInformation listing)
    {
        return listing.Status switch
        {
            ListingStatus.Back => 20,
            ListingStatus.Decreased => 11,
            ListingStatus.Increased => 11,
            ListingStatus.New => 20,
            _ => 11
        };
    }

    public static string ConvertDeltaToSymbol(ListingInformation listing)
    {
        return listing.Status switch
        {
            ListingStatus.Back => Symbols.BackInList,
            ListingStatus.Increased => Symbols.Up,
            ListingStatus.Decreased => Symbols.Down,
            ListingStatus.New => Symbols.Flag,
            ListingStatus.Unchanged => Symbols.Same,
            _ => Symbols.Minus
        };
    }

    public static string ConvertDeltaToString(ListingInformation listing)
    {
        return listing.Delta.HasValue
            ? Math.Abs(listing.Delta.Value).ToString()
            : string.Empty;
    }

    public static Color ConvertDeltaSymbolColour(ListingInformation listing)
    {
        return listing.Status switch
        {
            ListingStatus.Increased => Colours.GreenColour,
            ListingStatus.Decreased => Colours.RedColour,
            ListingStatus.New or ListingStatus.Back => Colours.YellowColour,
            _ => Colours.GreyColour
        };
    }
}