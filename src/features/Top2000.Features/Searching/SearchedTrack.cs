namespace Top2000.Features.Searching;

public class SearchedTrack
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public int RecordedYear { get; set; }

    public int? Position { get; set; }

    public int LatestEdition { get; set; }

    public string PositionInLatestEdition => $"{LatestEdition}: {Position?.ToString() ?? "-"}";
}