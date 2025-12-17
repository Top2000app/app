namespace Top2000.Features.Editions;

public class Edition
{
    public required int Year { get; init; }

    public DateTime LocalStartDateAndTime => DateTime.SpecifyKind(StartUtcDateAndTime, DateTimeKind.Utc).ToLocalTime();

    public DateTime LocalEndDateAndTime => DateTime.SpecifyKind(EndUtcDateAndTime, DateTimeKind.Utc).ToLocalTime();

    public required DateTime StartUtcDateAndTime { get; init; }

    public required DateTime EndUtcDateAndTime { get; init; }

    public required bool HasPlayDateAndTime { get; init; }
}
