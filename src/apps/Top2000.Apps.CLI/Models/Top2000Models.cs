namespace Top2000.Apps.CLI.Models;

public record Song(
    int Position,
    string Title,
    string Artist,
    int Year,
    string? PreviousPosition = null);

public record Artist(
    string Name,
    int SongCount,
    int HighestPosition);

public record YearStatistics(
    int Year,
    int SongCount);

public record DecadeStatistics(
    string Decade,
    int SongCount);
