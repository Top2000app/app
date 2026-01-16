using System.Collections.Specialized;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Top2000.Features;
using Top2000.Features.Editions;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Top2000Services _top2000Services;

    public MainWindowViewModel(Top2000Services services)
    {
        _top2000Services = services;
    }

    [ObservableProperty]
    public List<TrackListingViewModel> listings;
    
    [ObservableProperty]
    public Edition? selectedEdition;
    
    // public MainWindowViewModel(Top2000Services services)
    // {
    //     _services = services;
    //     Loaded();
    // }
    
    [RelayCommand]
    public async void LoadedAsync()
    {
        await _top2000Services.InitialiseDataAsync();

        await InitialiseViewModelAsync();
    }
    
    
    public async Task InitialiseViewModelAsync()
    {
        var editions = await _top2000Services.AllEditionsAsync();
        this.SelectedEdition = editions.First();
       // this.SelectedEditionYear = this.SelectedEdition.Year;
       // this.Editions.ClearAddRange(editions);

        await this.LoadAllListingsAsync();
    }

    public async Task InitialiseViewModelAsync(Edition newEdition)
    {
        this.SelectedEdition = newEdition;
        //this.SelectedEditionYear = this.SelectedEdition.Year;

        await this.LoadAllListingsAsync();
    }

    public async Task LoadAllListingsAsync()
    {
        if (this.SelectedEdition is null)
        {
            return;
        }

        var result = await _top2000Services.AllListingsOfEditionAsync(this.SelectedEdition.Year);

        var listingsFromdb = result
            .Select(x => new TrackListingViewModel
            {
                TrackId = x.TrackId,
                Artist = x.Artist,
                Title = x.Title,
                Delta = TrackListingViewModel.ConvertDeltaToString(x),
                DeltaFontSize = TrackListingViewModel.ConvertDeltaFontSize(x),
                PositionString = x.Position.ToString(),
                Position = x.Position,
                DeltaSymbol = TrackListingViewModel.ConvertDeltaToSymbol(x),
                DeltaSymbolColour = new SolidColorBrush(TrackListingViewModel.ConvertDeltaSymbolColour(x)),
                LocalPlayDateTime = x.PlayUtcDateAndTime.ToLocalTime()
            })
            // .GroupBy(x => Position(x, result.Count))
            .ToList();

     //   this.CountOfItems = result.Count;
        this.Listings = listingsFromdb;

     //  this.SelectedListing = null;
    }
    
    private static string Position(TrackListingViewModel listing, int countOfItems)
    {
        if (listing.Position < 100)
        {
            return "1 - 100";
        }

        if (countOfItems > 2000)
        {
            if (listing.Position >= 2400)
            {
                return "2400 - 2500";
            }
        }
        else if (listing.Position >= 1900)
        {
            return "1900 - 2000";
        }

        int num = listing.Position / 100 * 100;
        int value = num + 100;
        return $"{num} - {value}";
    }
}

public class TrackListingViewModel
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
            ? Math.Abs(track.Delta).ToString()
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

public static class Colours
{
    public static readonly Color YellowColour = Color.FromRgb(255, 192, 0);
    public static readonly Color RedColour = Color.FromRgb(221, 48, 57);
    public static readonly Color GreenColour = Color.FromRgb(112, 173, 71);
    public static readonly Color GreyColour = Color.FromRgb(103, 103, 103);
}

public static class Symbols
{
    public static string RadioButtonOpen => "\xe836";

    public static string RadioButtonChecked => "\xe837";

    public static string Overview => "\xe242";

    public static string Search => "\xe8b6";

    public static string Back => "\xe5c4";

    public static string Settings => "\xe8b8";

    public static string Menu => "\xe5d2";

    public static string FilterList => "\xe152";

    public static string Clock => "\xe192";

    public static string Today => "\xf217";

    public static string Flag => "\xe153";

    public static string Up => "\xe316";

    public static string Down => "\xe313";

    public static string BackInList => "\xe042";

    public static string Minus => "\xe15b";

    public static string Same => "\xe25d";

    public static string Video => "\xe63a";

    public static string New => "\xe05e";

    public static string Options => "\xe429";
}



public class ObservableList<TItem> : List<TItem>, INotifyCollectionChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Removed all the items in the list, add the list of items and notify the observers.
    /// </summary>
    /// <param name="items">Items to add</param>
    public void ClearAddRange(IEnumerable<TItem> items)
    {
        this.Clear();
        items.ToList().ForEach(this.Add);

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

public class ObservableGroupedList<TKey, TItem> : ObservableList<IGrouping<TKey, TItem>> { }

