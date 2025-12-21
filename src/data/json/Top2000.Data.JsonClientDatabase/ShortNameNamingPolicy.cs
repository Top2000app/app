using System.Text.Json;

namespace Top2000.Data.JsonClientDatabase;

public sealed class ShortNameNamingPolicy : JsonNamingPolicy
{
    private readonly Dictionary<string, string> _map = new()
    {
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
    };

    public override string ConvertName(string name)
        => _map.GetValueOrDefault(name, name);
}