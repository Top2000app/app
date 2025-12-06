namespace Top2000.Features.Searching;

public class SearchedTrack
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public required string Artist { get; init; }

    public required int RecordedYear { get; init; }

    public int? Position { get; init; }

    public required int LatestEdition { get; init; }

    public string PositionInLatestEdition => $"{LatestEdition}: {Position?.ToString() ?? "-"}";
}