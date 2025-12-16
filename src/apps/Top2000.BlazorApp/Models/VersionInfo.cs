using System.Text.Json.Serialization;

namespace Top2000.BlazorApp.Models;

public class VersionInfo
{
    [JsonPropertyName("latestEdition")]
    public int LatestEdition { get; set; }

    [JsonPropertyName("editions")]
    public List<int> Editions { get; set; } = new();
}
