namespace Top2000.Data.JsonClientDatabase.Models;

public class Edition
{
    public required int Year { get; init; }
    public required DateTime StartUtcDateAndTime { get; init; }
    public required DateTime EndUtcDateAndTime { get; init; }
    public required bool HasPlayDateAndTime { get; init; }
}