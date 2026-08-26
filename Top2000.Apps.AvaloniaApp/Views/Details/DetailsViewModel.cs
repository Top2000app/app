using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Apps.AvaloniaApp.Views.Details.TrackDetails;
using Top2000.Features;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.Views.Details;

public class DesignDetailsViewModel : DetailsViewModel
{
    public DesignDetailsViewModel() : base(new MockupTop2000Services())
    {
        
    }    
}

public partial class DetailsViewModel : ObservableObject, IShowTrackDetails
{
    private readonly ITop2000Services _top2000Services;

    [ObservableProperty]
    public partial bool IsListingSelected { get; set; }
    
    [ObservableProperty]
    public partial TrackDetailsViewModel? SelectedListing { get; set; }
    
    public DetailsViewModel(ITop2000Services top2000Services)
    {
        _top2000Services = top2000Services;
    }
    
    public async Task ShowTrackDetailsAsync(int trackId)
    {
      var details = await _top2000Services.TrackDetailsAsync(trackId);
      var trackListings = details.Listings
          .Select(Transform)
          .ToList();

      var orderedListings = trackListings
          .Where(x => x.Position.HasValue)
          .OrderBy(x => x.Position)
          .ThenBy(x => x.Edition)
          .ToList();

      var trackListingsForGraph = trackListings
          .OrderBy(x => x.Edition)
          .Select(x => new ObservablePoint
          {
             X = x.Edition,
             Y = x.Position,
          })
          .ToArray();

      var xxx = trackListingsForGraph.Select(x => (int?)x.Y).ToList();

      var zoomRange = TrackDetailsViewModel.GetZoomRange(xxx);

      if (SelectedListing is null)
      {
          IsListingSelected = true;

          SelectedListing = new TrackDetailsViewModel
          {
            //  ParentMainWindowViewModel = this,
              Artist =  details.Artist,
              Title = details.Title,
              RecordedYear = details.RecordedYear,
              Listings = trackListings,
              //SelectedListing = trackListings.First(x => x.Edition == SelectedEdition!.),
              LatestEditionPosition = trackListings.Last(x => x.Position.HasValue)?.Position.ToString() ?? "-",
              Highest = orderedListings.Last(),
              Lowest = orderedListings.First(),
              YMax = zoomRange.max,
              YMin = zoomRange.min,
            //  Positions = trackListingsForGraph.ToArray(),
              Series =
              [
                  new LineSeries<ObservablePoint>
                  {
                      Values = trackListingsForGraph,
                      GeometrySize = 6
                  }
              ]
          };
      }
      else
      {
          SelectedListing.YMax = zoomRange.max;
          SelectedListing.YMin = zoomRange.min;
          SelectedListing.Series =
          [
              new LineSeries<ObservablePoint>
              {
                  Values = trackListingsForGraph,
                  GeometrySize = 6
              }
          ];
      }
    }
    
    private static TrackDetailsListingViewModel Transform(ListingInformation x)
    {
        return new TrackDetailsListingViewModel
        {
            Edition = x.Edition,
            Delta = x.Delta?.ToString() ?? "",
            DeltaFontSize = TrackDetailsListingViewModel.ConvertDeltaFontSize(x),
            DeltaSymbol = TrackDetailsListingViewModel.ConvertDeltaToSymbol(x),
            DeltaSymbolColour =
                new SolidColorBrush(TrackDetailsListingViewModel.ConvertDeltaSymbolColour(x)),
            Position = x.Position,
            Status = x.Status
        };
    }
}