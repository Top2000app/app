using System.Text.Json;
using Top2000.BlazorApp.Models;
using Top2000.Features.Listing;

namespace Top2000.BlazorApp.Services;

public class TrackService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly Dictionary<int, List<TrackListing>> _trackListings = [];
    
    public TrackService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task InitializeAsync()
    {
        IsBackgroundLoading = true;
        var versionInfo = await LoadVersionInfoAsync();
        LatestEdition = versionInfo.LatestEdition;
       // foreach (var edition in versionInfo.Editions)
      //  {
            var listings = await LoadEditionDataAsync(LatestEdition);
            _trackListings.Add(LatestEdition, listings);
       // }

        IsBackgroundLoading = false;
    }

    public bool IsBackgroundLoading { get; private set; }
    public int LatestEdition { get; private set; }

    public List<TrackListing> ByEdition(int edition)
    {
        return _trackListings[edition];
    }
    
    //
    // public async Task<List<TrackListing>> GetTracksAsync(int year)
    // {
    //     // Make sure we're initialized
    //     // await InitializeAsync();
    //     
    //     // For now, just convert the ListingInformation to TrackListing
    //     // This is a simplified approach - in a real scenario you might want to store both
    //     return _trackListings
    //         .Where(x => x.Key == year)
    //         .SelectMany(x => x.Value)
    //         .OrderBy(x => x.Position)
    //         .ToList();
    // }

    public async Task<Models.Track?> GetTrackAsync(int year, int trackNumber)
    {
        await InitializeAsync();
        
        if (_trackListings.TryGetValue(year, out var tracks))
        {
            var track = tracks.FirstOrDefault(t => t.TrackId == trackNumber);
            if (track != null)
            {
                return new Models.Track
                {
                    Year = year,
                    Number = track.Position,
                    Title = track.Title,
                    Artist = track.Artist,
                    TrackId = track.TrackId
                };
            }
        }
        
        return null;
    }
    
    private async Task<VersionInfo> LoadVersionInfoAsync()
    {
        var json = await _httpClient.GetStringAsync("data/version.json");
        
        return JsonSerializer.Deserialize<VersionInfo>(json, JsonSerializerOptions)
            ?? throw new InvalidOperationException("Unable to deserialize version info.");
    }
    
    private async Task<List<TrackListing>> LoadEditionDataAsync(int edition)
    {
        var json = await _httpClient.GetStringAsync($"data/{edition}.json");
        
        return JsonSerializer.Deserialize<List<TrackListing>>(json, JsonSerializerOptions)
            ?? throw new InvalidOperationException($"Unable to deserialize edition data for edition {edition}.");
    }
}

