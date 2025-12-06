namespace Top2000.Data.Csv.Models;

public class Track
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Artist { get; init; } = string.Empty;

    public int RecordedYear { get; init; } = 1;

    public DateTime? LastPlayUtc { get; init; }

    public int FirstEdition { get; init; }
}