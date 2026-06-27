using Avalonia.Media;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Assets;

public static class SymbolsExtensions
{
    extension(TrackListingDeltaType type)
    {
        public int ToFontSize()
        {
            return type switch
            {
                TrackListingDeltaType.NoChange => 15,
                TrackListingDeltaType.Increased => 11,
                TrackListingDeltaType.Decreased => 11,
                TrackListingDeltaType.New => 20,
                TrackListingDeltaType.Recurring => 20,
                _ => 11
            };
        }
        
        public string ToSymbol()
        {
            return type switch
            {
                TrackListingDeltaType.NoChange => Symbols.Same,
                TrackListingDeltaType.Increased => Symbols.Up,
                TrackListingDeltaType.Decreased => Symbols.Down,
                TrackListingDeltaType.New => Symbols.Flag,
                TrackListingDeltaType.Recurring => Symbols.BackInList,
                _ => Symbols.Same
            };
        }

        public SolidColorBrush ToBrush()
        {
            var colour = type switch
            {
                TrackListingDeltaType.NoChange => Colours.GreyColour,
                TrackListingDeltaType.Increased => Colours.GreenColour,
                TrackListingDeltaType.Decreased => Colours.RedColour,
                TrackListingDeltaType.New => Colours.YellowColour,
                TrackListingDeltaType.Recurring => Colours.YellowColour,
                _ => Colours.GreyColour
            };
            
            return new SolidColorBrush(colour);
        }
    }
}