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
        return Templates[((ITrackListingViewModel)param!).HeaderName].Build(param); 
    }

    public bool Match(object? data)
    {
        return data is ITrackListingViewModel;
    }
}

public interface ITrackListingViewModel
{
    string HeaderName { get; }
    bool IsHeader { get;  }
}

public class TrackListingPlayTimeGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPlayTimeGroup);
    public required string GroupName { get; init; }
    public bool IsHeader => true;
}

public class TrackListingPositionGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPositionGroup);
    public required string GroupName { get; init; }
    public bool IsHeader => true;
}

public class TrackListingViewModel : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingViewModel);
    public bool IsHeader => false;
    
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