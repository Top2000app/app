using System.Text.Json;

namespace Top2000.Data.ClientDatabase;

public class OnlineDataSource : ISource
{
    private readonly HttpClient _httpClient;

    public OnlineDataSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ImmutableSortedSet<string>> ExecutableScriptsAsync(ImmutableSortedSet<string> journals)
    {
        if (journals.IsEmpty)
        {
            return [];
        }

        var latestVersion = journals[^1]
            .Split('-')[0];

        var content = await TryGetAsyncForUpgradesAsync(latestVersion).ConfigureAwait(false);

        if (content is null)
        {
            return [];
        }

        var contents = JsonSerializer.Deserialize<IEnumerable<string>>(content);

        return contents?.ToImmutableSortedSet() ?? [];
    }

    public async Task<SqlScript> ScriptContentsAsync(string scriptName)
    {
        var requestUri = $"sql/{scriptName}";

        var response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var contents = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return new SqlScript(scriptName, contents);
    }

    private async Task<string?> TryGetAsyncForUpgradesAsync(string latestVersion)
    {
        try
        {
            var requestUri = $"versions/{latestVersion}/upgrades";
            var response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}
