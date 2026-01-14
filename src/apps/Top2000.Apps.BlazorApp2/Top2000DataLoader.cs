using Top2000.Features.Json;

namespace Top2000.Apps.MudBlazorApp;

public class Top2000DataLoader : IDataLoader
{
    public Top2000DataLoader()
    {

    }
    
    public Task<Stream> LoadDataVersionAsync()
    {
        var stream = typeof(Top2000DataLoader).Assembly.GetManifestResourceStream(
            "Top2000.Apps.BlazorApp2.wwwroot.data.version.json")
            ?? throw new InvalidOperationException("Could not find embedded resource 'Data.version.json'");
        
        return Task.FromResult(stream);
    }

    public Task<Stream> LoadEditionDataAsync(int edition)
    {
        var stream = typeof(Top2000DataLoader).Assembly.GetManifestResourceStream(
                         $"Top2000.Apps.BlazorApp2.wwwroot.data.{edition}.json")
                     ?? throw new InvalidOperationException("Could not find embedded resource 'Data.version.json'");
        
        return Task.FromResult(stream);
    }

}