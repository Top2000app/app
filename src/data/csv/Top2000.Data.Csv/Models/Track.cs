using System;

namespace Top2000.Data.Csv.Models;

public class Track
{
    public required int Id { get; init; }

    public required string Title { get; init; } = string.Empty;

    public required string Artist { get; init; } = string.Empty;

    public required int RecordedYear { get; init; } = 1;

    public required DateTime? LastPlayUtc { get; init; }

    public required int FirstEdition { get; init; }
}