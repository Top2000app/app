using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;

namespace Top2000.Apps.CLI.Commands.SpotifyHelper;

public class SpotifyCommand(Top2000DbContext db)
    : CommandBase("spotify", "spotify commands")
{
    private const string ClientId =  "";
    private const string ClientSecret = "";

    private const string OutputFile = "spotify.csv";

    // Deliberately slow: ~40 requests/minute.
    private static readonly TimeSpan RequestDelay =
        TimeSpan.FromMilliseconds(1500);

    protected override async Task ExecuteAsync(
        ParseResult result,
        CancellationToken token)
    {
        using var http = new HttpClient();

        http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Top2000-SpotifyMatcher/1.0");

        // ------------------------------------------------------------
        // Get Spotify access token
        // ------------------------------------------------------------

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));

        using var tokenRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "https://accounts.spotify.com/api/token");

        tokenRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);

        tokenRequest.Content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

        using var tokenResponse =
            await http.SendAsync(tokenRequest, token);

        tokenResponse.EnsureSuccessStatusCode();

        var tokenJson =
            await tokenResponse.Content.ReadAsStringAsync(token);

        var tokenData =
            JsonSerializer.Deserialize<TokenResponse>(tokenJson)
            ?? throw new Exception("Could not read Spotify token.");

        var accessToken = tokenData.AccessToken;

        // ------------------------------------------------------------
        // Load tracks
        // ------------------------------------------------------------

        var tracks = await db.Tracks
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(token);

        Console.WriteLine($"Found {tracks.Count} tracks.");

        // ------------------------------------------------------------
        // Load existing results so the command can be restarted
        // ------------------------------------------------------------

        var results = new Dictionary<int, Result>();

        if (File.Exists(OutputFile))
        {
            foreach (var line in await File.ReadAllLinesAsync(
                         OutputFile,
                         token))
            {
                if (line.StartsWith("Id;"))
                    continue;

                var parts = line.Split(';');

                if (parts.Length < 6)
                    continue;

                if (!int.TryParse(parts[0], out var id))
                    continue;

                if (!double.TryParse(
                        parts[5],
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var confidence))
                {
                    continue;
                }

                results[id] = new Result
                {
                    Id = id,
                    Title = parts[1],
                    Artist = parts[2],
                    RecordedYear = int.TryParse(parts[3], out var year)
                        ? year
                        : 0,
                    SpotifyTrackId =
                        string.IsNullOrWhiteSpace(parts[4])
                            ? null
                            : parts[4],
                    Confidence = confidence
                };
            }
        }

        Console.WriteLine(
            $"Already processed: {results.Count}");

        // ------------------------------------------------------------
        // Process tracks
        // ------------------------------------------------------------

        var processed = 0;

        foreach (var track in tracks)
        {
            token.ThrowIfCancellationRequested();

            if (results.ContainsKey(track.Id))
                continue;

            processed++;

            Console.Write(
                $"[{processed}/{tracks.Count - results.Count}] " +
                $"{track.Artist} - {track.Title} ... ");

            var match = await FindSpotifyTrack(
                http,
                accessToken,
                track,
                token);

            results[track.Id] = new Result
            {
                Id = track.Id,
                Title = track.Title,
                Artist = track.Artist,
                RecordedYear = track.RecordedYear,
                SpotifyTrackId = match?.Id,
                Confidence = match?.Confidence ?? 0
            };

            if (match == null)
            {
                Console.WriteLine("NO MATCH");
            }
            else
            {
                Console.WriteLine(
                    $"{match.Id} ({match.Confidence:P0})");
            }

            // Save after every track so Ctrl+C doesn't lose progress.
            await WriteResults(results.Values, token);

            // Don't make the next request immediately.
            await Task.Delay(RequestDelay, token);
        }

        Console.WriteLine();
        Console.WriteLine($"Finished. Results: {OutputFile}");
    }

    private static async Task<Match?> FindSpotifyTrack(
        HttpClient http,
        string accessToken,
        Track track,
        CancellationToken token)
    {
        // First search: strict artist + title.
        var query =
            $"track:\"{Clean(track.Title)}\" " +
            $"artist:\"{Clean(track.Artist)}\"";

        var candidates =
            await Search(http, accessToken, query, token);

        // If Spotify didn't find anything, try a less restrictive search.
        if (candidates.Count == 0)
        {
            query =
                $"\"{Clean(track.Title)}\" \"{Clean(track.Artist)}\"";

            candidates =
                await Search(http, accessToken, query, token);
        }

        if (candidates.Count == 0)
            return null;

        return candidates
            .Select(x => new Match
            {
                Id = x.Id,
                Confidence = Score(track, x)
            })
            .OrderByDescending(x => x.Confidence)
            .First();
    }

    private static async Task<List<SpotifyTrack>> Search(
        HttpClient http,
        string accessToken,
        string query,
        CancellationToken token)
    {
        var url =
            "https://api.spotify.com/v1/search" +
            $"?q={Uri.EscapeDataString(query)}" +
            "&type=track" +
            "&limit=10";

        while (true)
        {
            using var request =
                new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken);

            using var response =
                await http.SendAsync(request, token);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var seconds =
                    response.Headers.RetryAfter?.Delta?.TotalSeconds
                    ?? 30;

                Console.Write(
                    $"[rate limited, waiting {seconds:0}s] ");

                await Task.Delay(
                    TimeSpan.FromSeconds(seconds + 1),
                    token);

                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.Write(
                    $"[Spotify {(int)response.StatusCode}] ");

                return [];
            }

            var json =
                await response.Content.ReadAsStringAsync(token);

            var result =
                JsonSerializer.Deserialize<SearchResponse>(json);

            return result?.Tracks?.Items ?? [];
        }
    }

    private static double Score(
        Track source,
        SpotifyTrack spotify)
    {
        var titleScore =
            Similarity(
                Normalize(source.Title),
                Normalize(spotify.Name));

        var spotifyArtist =
            string.Join(
                " ",
                spotify.Artists.Select(x => x.Name));

        var artistScore =
            Similarity(
                Normalize(source.Artist),
                Normalize(spotifyArtist));

        // Year is deliberately a small factor because Spotify may
        // return the release year of a remaster rather than the
        // original recording year.
        var yearScore = 0.5;

        if (spotify.Album?.ReleaseDate?.Length >= 4 &&
            int.TryParse(
                spotify.Album.ReleaseDate[..4],
                out var spotifyYear))
        {
            var difference =
                Math.Abs(source.RecordedYear - spotifyYear);

            yearScore = difference switch
            {
                0 => 1.0,
                1 => 0.8,
                2 => 0.6,
                3 => 0.4,
                _ => 0.1
            };
        }

        return
            titleScore * 0.55 +
            artistScore * 0.35 +
            yearScore * 0.10;
    }

    private static string Normalize(string value)
    {
        value = value.ToLowerInvariant();

        value = value
            .Replace("feat.", " ")
            .Replace("feat", " ")
            .Replace("ft.", " ")
            .Replace("ft", " ")
            .Replace("&", " ");

        // Remove punctuation.
        value = new string(
            value
                .Select(c =>
                    char.IsLetterOrDigit(c) ? c : ' ')
                .ToArray());

        return string.Join(
            " ",
            value.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));
    }

    private static string Clean(string value) =>
        value.Replace("\"", "").Trim();

    private static double Similarity(
        string a,
        string b)
    {
        if (a == b)
            return 1;

        if (string.IsNullOrWhiteSpace(a) ||
            string.IsNullOrWhiteSpace(b))
            return 0;

        var distance = Levenshtein(a, b);

        return 1.0 -
               (double)distance /
               Math.Max(a.Length, b.Length);
    }

    private static int Levenshtein(
        string a,
        string b)
    {
        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];

        for (var j = 0; j <= b.Length; j++)
            previous[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;

            for (var j = 1; j <= b.Length; j++)
            {
                var cost =
                    a[i - 1] == b[j - 1] ? 0 : 1;

                current[j] = Math.Min(
                    Math.Min(
                        current[j - 1] + 1,
                        previous[j] + 1),
                    previous[j - 1] + cost);
            }

            (previous, current) = (current, previous);
        }

        return previous[b.Length];
    }

    private static async Task WriteResults(
        IEnumerable<Result> results,
        CancellationToken token)
    {
        await using var file = new StreamWriter(
            OutputFile,
            false,
            new UTF8Encoding(false));

        await file.WriteLineAsync(
            "Id;Title;Artist;RecordedYear;SpotifyTrackId;Confidence");

        foreach (var result in results.OrderBy(x => x.Id))
        {
            await file.WriteLineAsync(
                $"{result.Id};" +
                $"{Csv(result.Title)};" +
                $"{Csv(result.Artist)};" +
                $"{result.RecordedYear};" +
                $"{result.SpotifyTrackId ?? ""};" +
                $"{result.Confidence.ToString(
                    "0.000",
                    System.Globalization.CultureInfo.InvariantCulture)}");
        }
    }

    private static string Csv(string value) =>
        value
            .Replace(";", ",")
            .Replace("\r", " ")
            .Replace("\n", " ");

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    private sealed class SearchResponse
    {
        [JsonPropertyName("tracks")]
        public TrackContainer? Tracks { get; set; }
    }

    private sealed class TrackContainer
    {
        [JsonPropertyName("items")]
        public List<SpotifyTrack> Items { get; set; } = [];
    }

    private sealed class SpotifyTrack
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("artists")]
        public List<SpotifyArtist> Artists { get; set; } = [];

        [JsonPropertyName("album")]
        public SpotifyAlbum? Album { get; set; }
    }

    private sealed class SpotifyArtist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }

    private sealed class SpotifyAlbum
    {
        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
    }

    private sealed class Match
    {
        public string Id { get; init; } = "";
        public double Confidence { get; init; }
    }

    private sealed class Result
    {
        public int Id { get; init; }
        public string Title { get; init; } = "";
        public string Artist { get; init; } = "";
        public int RecordedYear { get; init; }
        public string? SpotifyTrackId { get; init; }
        public double Confidence { get; init; }
    }
}