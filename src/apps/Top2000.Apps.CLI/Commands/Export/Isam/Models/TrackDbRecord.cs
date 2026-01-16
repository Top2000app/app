using DownloaderApp;

namespace Top2000.Apps.CLI.Commands.Export.Isam;

public class TrackDbRecord
{
    public required int TrackId { get; init; }
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required int RecordedYear { get; init; }
    public required int HighestPosition { get; init; }
    public required int HighestEdition { get; init; }
    public required int LowestPosition { get; init; }
    public required int LowestEdition { get; init; }
    public required int FirstPosition { get; init; }
    public required int FirstEdition { get; init; }
    public required int LatestPosition { get; init; }
    public required int LatestEdition { get; init; }
    public required string LatestPlayLocalDateAndTime { get; init; }
    public required int Appearances { get; init; }
    public required int AppearancesPossible { get; init; }
    
    private static string ReplaceSpecialChars(string input)
    {
        return input
                .Replace("ä", "a", StringComparison.InvariantCulture)
                .Replace("á", "a", StringComparison.InvariantCulture)
                .Replace("à", "a", StringComparison.InvariantCulture)
                .Replace("ã", "a", StringComparison.InvariantCulture)
                .Replace("â", "a", StringComparison.InvariantCulture)

                .Replace("ê", "e", StringComparison.InvariantCulture)
                .Replace("ë", "e", StringComparison.InvariantCulture)
                .Replace("é", "e", StringComparison.InvariantCulture)
                .Replace("è", "e", StringComparison.InvariantCulture)
                .Replace("È", "E", StringComparison.InvariantCulture)

                .Replace("ö", "o", StringComparison.InvariantCulture)
                .Replace("ó", "o", StringComparison.InvariantCulture)
                .Replace("ò", "o", StringComparison.InvariantCulture)
                .Replace("ô", "o", StringComparison.InvariantCulture)
                .Replace("õ", "o", StringComparison.InvariantCulture)

                .Replace("ø", "o", StringComparison.InvariantCulture)
                .Replace("Ø", "O", StringComparison.InvariantCulture)

                .Replace("î", "i", StringComparison.InvariantCulture)
                .Replace("ï", "i", StringComparison.InvariantCulture)
                .Replace("í", "i", StringComparison.InvariantCulture)
                .Replace("ì", "i", StringComparison.InvariantCulture)
                .Replace("î", "i", StringComparison.InvariantCulture)

                .Replace("&", "+", StringComparison.InvariantCulture)
                .Replace(",", " ", StringComparison.InvariantCulture)
            ;
    }

    public string ToCsvString()
    {
        return "" +
               $"{TrackId}," +
               $"{Title}," +
               $"{Artist}," +
               $"{RecordedYear}," +
               $"{HighestPosition}," +
               $"{HighestEdition}," +
               $"{LowestPosition}," +
               $"{LowestEdition}," +
               $"{FirstPosition}," +
               $"{FirstEdition}," +
               $"{LatestPosition}," +
               $"{LatestEdition}," +
               $"{LatestPlayLocalDateAndTime}," +
               $"{Appearances}," +
               $"{AppearancesPossible}";
    }

    public static TrackDbRecord ToTrackDbRecord(int trackId, TrackDetails track)
    {
        return new TrackDbRecord
        {
            TrackId = trackId,
            Title = ReplaceSpecialChars(track.Title),
            Artist = ReplaceSpecialChars(track.Artist),
            RecordedYear = track.RecordedYear,
            HighestPosition = track.Highest.Position ?? throw new InvalidOperationException("Position should be available"),
            HighestEdition = track.Highest.Edition,
            LowestPosition = track.Lowest.Position ?? throw new InvalidOperationException("Position should be available"),
            LowestEdition = track.Lowest.Edition,
            FirstPosition = track.First.Position ?? throw new InvalidOperationException("Position should be available"),
            FirstEdition = track.First.Edition,
            LatestPosition = track.Latest.Position ?? throw new InvalidOperationException("Position should be available"),
            LatestEdition = track.Latest.Edition,
            Appearances = track.Appearances,
            AppearancesPossible = track.AppearancesPossible,
            LatestPlayLocalDateAndTime = track.Latest.PlayUtcDateAndTime.HasValue
                ? track.Latest.PlayUtcDateAndTime.Value.ToString("dd-MM-yyyy HH:mm") +
                  $"-{track.Latest.PlayUtcDateAndTime.Value.Hour + 1}:00"
                : "-",
        };
    }
}