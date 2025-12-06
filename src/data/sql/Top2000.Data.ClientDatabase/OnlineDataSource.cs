using System.Text.Json;

namespace Top2000.Data.ClientDatabase;

public class OnlineDataSource : ISource
{
    private readonly IHttpClientFactory httpClientFactory;

    public OnlineDataSource(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<ImmutableSortedSet<string>> ExecutableScriptsAsync(ImmutableSortedSet<string> journals)
    {
        if (journals.IsEmpty)
        {
            return [];
        }

        var latestVersion = journals[^1]
            .Split('-')[0];

        var content = await TryGetAsyncForUpgrades(latestVersion).ConfigureAwait(false);

        if (content is null)
        {
            return [];
        }

        var contents = JsonSerializer.Deserialize<IEnumerable<string>>(content);

        return contents?.ToImmutableSortedSet() ?? [];
    }

    public async Task<SqlScript> ScriptContentsAsync(string scriptName)
    {
        var httpClient = httpClientFactory.CreateClient("top2000");
        var requestUri = $"sql/{scriptName}";

        var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var contents = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return new SqlScript(scriptName, contents);
    }

    private async Task<string?> TryGetAsyncForUpgrades(string latestVersion)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("top2000");
            var requestUri = $"versions/{latestVersion}/upgrades";
            var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}
