using DownloaderApp;

namespace Top2000.Apps.CLI.Commands.Export.Isam;

public class TrackDbRecord
{
    public required short TrackId { get; init; }
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required short RecordedYear { get; init; }
    public required short HighestPosition { get; init; }
    public required short HighestEdition { get; init; }
    public required short LowestPosition { get; init; }
    public required short LowestEdition { get; init; }
    public required short FirstPosition { get; init; }
    public required short FirstEdition { get; init; }
    public required short LatestPosition { get; init; }
    public required short LatestEdition { get; init; }
    public required string LatestPlayLocalDateAndTime { get; init; }
    public required short Appearances { get; init; }
    public required short AppearancesPossible { get; init; }
    
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
            TrackId = (short)trackId,
            Title = ReplaceSpecialChars(track.Title),
            Artist = ReplaceSpecialChars(track.Artist),
            RecordedYear = (short)track.RecordedYear,
            HighestPosition = (short)(track.Highest?.Position ??
                                      throw new InvalidOperationException("Position should be available")),
            HighestEdition = (short)track.Highest.Edition,
            LowestPosition = (short)(track.Lowest?.Position ??
                                     throw new InvalidOperationException("Position should be available")),
            LowestEdition = (short)track.Lowest.Edition,
            FirstPosition = (short)(track.First.Position ??
                                    throw new InvalidOperationException("Position should be available")),
            FirstEdition = (short)track.First.Edition,
            LatestPosition = (short)(track.Latest.Position ??
                                     throw new InvalidOperationException("Position should be available")),
            LatestEdition = (short)track.Latest.Edition,
            LatestPlayLocalDateAndTime = track.Latest.PlayUtcDateAndTime.HasValue
                ? track.Latest.PlayUtcDateAndTime.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm") +
                  $"-{track.Latest.PlayUtcDateAndTime.Value.ToLocalTime().Hour + 1}:00"
                : "-",
            Appearances = (short)track.Appearances,
            AppearancesPossible = (short)track.AppearancesPossible
        };
    }
}