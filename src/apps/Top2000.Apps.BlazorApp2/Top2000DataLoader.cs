using Top2000.Features.Json;

namespace Top2000.Apps.MudBlazorApp;

public class Top2000DataLoader : IDataLoader
{
    private readonly HttpClient _httpClient;

    public Top2000DataLoader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public Task<Stream> LoadDataVersionAsync()
    {
        return _httpClient.GetStreamAsync("data/version.json");
    }

    public Task<Stream> LoadEditionDataAsync(int edition)
    {
        return _httpClient.GetStreamAsync($"data/{edition}.json");
    }

}