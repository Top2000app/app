using System.Text.Json;

namespace Top2000.Apps.CLI.Commands.Export.Json;

public sealed class ShortNameNamingPolicy : JsonNamingPolicy
{
    private readonly IReadOnlyDictionary<string, string> _map;

    public ShortNameNamingPolicy()
    {
        _map = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            // Example mappings per model type; keep unique per property name scope
            // Track
            ["Year"] = "y",
            ["StartUtcDateAndTime"] = "s",
            ["EndUtcDateAndTime"] = "e",
            ["HasPlayDateAndTime"] = "h",
            ["TrackId"] = "t",
            ["EditionId"] = "z",
            ["Position"] = "r",
            ["PlayUtcDateAndTime"] = "p",
            ["Id"] = "i",
            ["Title"] = "l",
            ["Artist"] = "a",
            ["RecordedYear"] = "c",
        };;
    }

    public override string ConvertName(string name)
        => _map.TryGetValue(name, out var shortName) ? shortName : name;
}