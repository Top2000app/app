namespace Top2000.BlazorApp.Services;

public class TrackService
{
    public async Task<List<Track>> GetTracksAsync(int edition)
    {
        await Task.Yield();

        return [];
    }
    public async Task<Track?> GetTrackAsync(int year, int number)
    { 
        await Task.Yield();
        return new Track { Year = year, Number = number };
    }
}

public class Track
{
    public int Year { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
}