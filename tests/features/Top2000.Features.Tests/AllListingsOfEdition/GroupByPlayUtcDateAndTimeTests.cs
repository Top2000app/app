using Top2000.Features.AllListingsOfEdition;

namespace Top2000.Features.Tests.AllListingsOfEdition;

[TestClass]
public class GroupByPlayUtcDateAndTimeTests
{
    [TestMethod]
    public void TracksAreGroupedPerHour()
    {
        var list = new HashSet<TrackListing> { CreateNewTrackListingWithPosition(new DateTime(2024, 9, 1, 10, 5, 1, DateTimeKind.Utc)) };

        var grouped = list
            .GroupByPlayUtcDateAndTime()
            .ToList();

        grouped[^1].Key.Should().Be(new DateTime(2024, 9, 1, 10, 0, 0, DateTimeKind.Utc));
    }

    [TestMethod]
    public void TrackOfTwoDifferentHoursGeneratedTwoGroups()
    {
        var list = new HashSet<TrackListing>
        {
            CreateNewTrackListingWithPosition(new DateTime(2024, 9, 1, 10, 5, 1, DateTimeKind.Utc)),
            CreateNewTrackListingWithPosition(new DateTime(2024, 9, 1, 10, 10, 1, DateTimeKind.Utc)),
            CreateNewTrackListingWithPosition(new DateTime(2024, 9, 2, 11, 12, 12, DateTimeKind.Utc))
        };

        var grouped = list
            .GroupByPlayUtcDateAndTime()
            .ToList();

        var firstGroup = grouped[0];
        firstGroup.Key.Should().Be(new DateTime(2024, 9, 1, 10, 0, 0, DateTimeKind.Utc));
        firstGroup.Should().HaveCount(2);

        var lastGroup = grouped[^1];
        lastGroup.Key.Should().Be(new DateTime(2024, 9, 2, 11, 0, 0, DateTimeKind.Utc));
        lastGroup.Should().HaveCount(1);
    }

    private static TrackListing CreateNewTrackListingWithPosition(DateTime playUtcDateAndTime)
    {
        return new()
        {
            Artist = "UnitTest",
            Delta = 0,
            IsRecurring = true,
            Position = 0,
            PlayUtcDateAndTime = playUtcDateAndTime,
            Title = "UnitTest",
            TrackId = 0,
        };
    }
}
