using System.Text.Json;
using Top2000.BlazorApp.Models;
using Top2000.Features.TrackInformation;
using Top2000.Features.AllListingsOfEdition;

namespace Top2000.BlazorApp.Services;

public class TrackService
{
    private readonly List<ListingInformation> _tracks = [];
    private readonly Dictionary<int, List<TrackListing>> _trackListings = [];
    private readonly IServiceProvider _serviceProvider;
    private readonly SemaphoreSlim _loadingSemaphore = new(1, 1);
    private VersionInfo? _versionInfo;
    private bool _isInitialized = false;
    private bool _isBackgroundLoading = false;
    
    public TrackService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        await _loadingSemaphore.WaitAsync();
        try
        {
            if (_isInitialized) return;

            Console.WriteLine("Starting TrackService initialization...");

            // Step 1: Load version.json
            _versionInfo = await LoadVersionInfoAsync();
            
            if (_versionInfo == null)
            {
                Console.WriteLine("Failed to load version.json, using fallback");
                // Create a fallback version info for 2024 if version.json fails
                _versionInfo = new VersionInfo 
                { 
                    LatestEdition = 2024, 
                    Editions = Enumerable.Range(1999, 26).ToList() 
                };
            }
            
            Console.WriteLine($"Version info loaded. Latest edition: {_versionInfo.LatestEdition}");
            
            // Step 2: Load the latest edition data
            Console.WriteLine($"Loading latest edition data for {_versionInfo.LatestEdition}...");
            await LoadEditionDataAsync(_versionInfo.LatestEdition);
            
            _isInitialized = true;
            Console.WriteLine("TrackService initialization complete. Starting background loading...");
            
            // Step 5: Start background loading of all other editions
            _ = Task.Run(LoadRemainingEditionsAsync);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during TrackService initialization: {ex.Message}");
            // Mark as initialized even on error to prevent infinite retry
            _isInitialized = true;
        }
        finally
        {
            _loadingSemaphore.Release();
        }
    }
    
    public List<ListingInformation> ByEdition(int edition)
    {
        return _tracks
            .Where(x => x.Edition == edition)
            .ToList();
    }
    
    public async Task<List<TrackListing>> GetTracksAsync(int year)
    {
        // Make sure we're initialized
        await InitializeAsync();
        
        // For now, just convert the ListingInformation to TrackListing
        // This is a simplified approach - in a real scenario you might want to store both
        return _trackListings
            .Where(x => x.Key == year)
            .SelectMany(x => x.Value)
            .OrderBy(x => x.Position)
            .ToList();
    }

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
    
    public bool IsBackgroundLoading => _isBackgroundLoading;
    
    public int LoadedEditionsCount => _trackListings.Count;
    
    public int TotalEditionsCount => _versionInfo?.Editions.Count ?? 0;
    
    public int? LatestEdition => _versionInfo?.LatestEdition;
    
    public List<int> AvailableEditions => _versionInfo?.Editions ?? new List<int>();
    
    private async Task<VersionInfo?> LoadVersionInfoAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var json = await httpClient.GetStringAsync("data/version.json");
            return JsonSerializer.Deserialize<VersionInfo>(json);
        }
        catch (Exception ex)
        {
            // Log error - for now just return null
            Console.WriteLine($"Error loading version.json: {ex.Message}");
            return null;
        }
    }
    
    private async Task LoadEditionDataAsync(int edition)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var json = await httpClient.GetStringAsync($"data/{edition}.json");
            var trackData = JsonSerializer.Deserialize<List<TrackData>>(json);
            
            if (trackData != null)
            {
                var listingInfo = trackData.Select(td => td.ToListingInformation(edition)).ToList();
                var trackListings = trackData.Select(td => td.ToTrackListing()).ToList();
                
                await _loadingSemaphore.WaitAsync();
                try
                {
                    // Remove existing data for this edition and add new data
                    _tracks.RemoveAll(x => x.Edition == edition);
                    _tracks.AddRange(listingInfo);
                    
                    // Store track listings for UI components
                    _trackListings[edition] = trackListings;
                }
                finally
                {
                    _loadingSemaphore.Release();
                }
            }
        }
        catch (Exception ex)
        {
            // Log error - for now just print to console
            Console.WriteLine($"Error loading edition {edition}: {ex.Message}");
        }
    }
    
    private async Task LoadRemainingEditionsAsync()
    {
        if (_versionInfo == null) return;
        
        _isBackgroundLoading = true;
        
        try
        {
            var editionsToLoad = _versionInfo.Editions
                .Where(edition => edition != _versionInfo.LatestEdition)
                .ToList();
            
            Console.WriteLine($"Starting background loading of {editionsToLoad.Count} remaining editions...");
            
            // Load editions in parallel but limit concurrency
            var semaphore = new SemaphoreSlim(3, 3); // Limit to 3 concurrent downloads
            var tasks = editionsToLoad.Select(async edition =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await LoadEditionDataAsync(edition);
                    Console.WriteLine($"Background loaded edition {edition}. Progress: {LoadedEditionsCount}/{TotalEditionsCount}");
                }
                finally
                {
                    semaphore.Release();
                }
            });
            
            await Task.WhenAll(tasks);
            Console.WriteLine("Background loading complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during background loading: {ex.Message}");
        }
        finally
        {
            _isBackgroundLoading = false;
        }
    }
}

