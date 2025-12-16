namespace Top2000.BlazorApp.Services;

public class EditionService
{
    private readonly TrackService _trackService;
    
    public EditionService(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    public async Task<int> GetLatestEditionAsync()
    {
        // Wait for TrackService to be initialized if it's not already
        if (_trackService.LatestEdition == null)
        {
            await _trackService.InitializeAsync();
        }
        
        return _trackService.LatestEdition ?? DateTime.Now.Year;
    }
}