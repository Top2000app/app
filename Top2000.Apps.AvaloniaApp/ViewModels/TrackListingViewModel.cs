using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Metadata;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.ViewModels;


public class TrackListingViewModelTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<string, IDataTemplate> Templates {get;} = new();

    public Control? Build(object? param)
    {
        return param is TrackListingViewModelGroup 
            ? Templates["group"].Build(param) 
            : Templates["item"].Build(param);
    }

    public bool Match(object? data)
    {
        return data is TrackListingViewModel or TrackListingViewModelGroup;
    }
}

public interface ITrackListingViewModel
{
    
}

public class TrackListingViewModelGroup : ITrackListingViewModel
{
    public required string GroupName { get; init; }
}

public class TrackListingViewModel : ITrackListingViewModel
{
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


    public static string ConvertDeltaToString(TrackListing track)
    {
        return track.Delta != 0
            ? Math.Abs(track.Delta).ToString()
            : string.Empty;
    }
}