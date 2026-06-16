using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Apps.AvaloniaApp.Views;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

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
}

public class TrackListingPlayDateTimeGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPlayDateTimeGroup);
    public required string GroupName { get; init; }
    public required DateTime PlayTime { get; init; }
    
    public static string MakeNiceDateTimeString(DateTime dateTime)
    {
        var localTime = dateTime.ToLocalTime();
        return $"{localTime:dddd dd MMM HH:00}-{localTime.AddHours(1):H:00}";
    }
}

public class TrackListingPlayDateGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPlayDateGroup);
    public required string GroupName { get; init; }
    
    public required List<TrackListingPlayTimeItem> PlayTimes { get; init; } 
}

public class TrackListingPositionGroupHeader : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPositionGroupHeader);
    public required string  ItemText { get; init; }
    
    public required List<TrackListingPosition> PositionGroups { get; init; }
}

public partial class TrackListingPosition : ObservableObject, ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPosition);
    public required string ItemText { get; init; }
    public required IHandleGroupSelection NewGroupHandler { get; init; }
    public required TrackListingPositionGroup Parent { get; init; }

    [RelayCommand]
    private void GotoPosition()
    {
        NewGroupHandler.SelectingGroup(Parent);
    }
}

public partial class TrackListingPlayTimeItem : ObservableObject, ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPlayTimeItem);
    public required string Time { get; init; }
    public bool IsHeader => true;
    public required  ITrackListingViewModel Parent { get; init; }
    
    public required IHandleGroupSelection NewGroupHandler { get; init; }

    [RelayCommand]
    private void GotoPlayTime()
    {
        NewGroupHandler.SelectingGroup(Parent);
    }
}

public class TrackListingPositionGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPositionGroup);
    public required string GroupName { get; init; }
    
    public required int PositionRangStart { get; init; }
    public required int PositionRangEnd { get; init; }
}

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