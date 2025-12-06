namespace Top2000.Data.ClientDatabase.Models;

public class Edition
{
    [Key]
    public required int Year { get; init; }
    public required DateTime StartUtcDateAndTime { get; init; }
    public required DateTime EndUtcDateAndTime { get; init; }
    public required bool HasPlayDateAndTime { get; init; }
}
