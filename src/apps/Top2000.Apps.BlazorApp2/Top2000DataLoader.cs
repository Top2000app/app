using Top2000.Features.Json;

namespace Top2000.Apps.MudBlazorApp;

public class Top2000DataLoader : IDataLoader
{
    private readonly HttpClient _httpClient;

    public Top2000DataLoader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public Task<Stream> LoadDataAsync(CancellationToken cancellationToken = default)
    {
        return _httpClient.GetStreamAsync("data/top2000.json", cancellationToken);
    }
}