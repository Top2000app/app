
using Top2000.Features.Listing;

namespace Top2000.Features.Tests.AllListingsOfEdition;

[TestClass]
public class GroupByPositionTests
{
    [TestMethod]
    public void Position1IsPutInThe1100Group()
    {
        var list = new HashSet<TrackListing> { CreateNewTrackListingWithPosition(1) };

        var grouped = list
            .GroupByPosition()
            .ToList();

        grouped[0].Key.Should().Be("1 - 100");
    }

    [TestMethod]
    public void Position99IsPutInThe1100Group()
    {
        var list = new HashSet<TrackListing> { CreateNewTrackListingWithPosition(99) };

        var grouped = list
            .GroupByPosition()
            .ToList();

        grouped[0].Key.Should().Be("1 - 100");
    }

    [TestMethod]
    public void Position100IsPutInThe100101Group()
    {
        var list = new HashSet<TrackListing> { CreateNewTrackListingWithPosition(100) };

        var grouped = list
            .GroupByPosition()
            .ToList();

        grouped[0].Key.Should().Be("100 - 200");
    }

    [TestMethod]
    public void Position2000IsPutInThe19992000Group()
    {
        var list = new HashSet<TrackListing> { CreateNewTrackListingWithPosition(2000) };

        var grouped = list
            .GroupByPosition()
            .ToList();

        grouped[0].Key.Should().Be("1900 - 2000");
    }

    [TestMethod]
    public void Position2000IsPutInThe20002100GroupWhenMoreThen2000Tracks()
    {
        var tracks = Enumerable
            .Range(1, 2005)
            .Select(x => CreateNewTrackListingWithPosition(x))
            .ToList();

        var list = new HashSet<TrackListing>(tracks);

        var grouped = list
            .GroupByPosition()
            .ToList();

        var lastGroup = grouped[^1];
        lastGroup.Key.Should().Be("2000 - 2100");
        lastGroup.Should().Contain(x => x.Position == 2000);
    }

    [TestMethod]
    public void Position2500IsPutInThe24002500Group()
    {
        var tracks = Enumerable
           .Range(1, 2501)
           .Select(x => CreateNewTrackListingWithPosition(x))
           .ToList();

        var list = new HashSet<TrackListing>(tracks);

        var grouped = list
            .GroupByPosition()
            .ToList();

        var lastGroup = grouped[^1];
        lastGroup.Key.Should().Be("2400 - 2500");
        lastGroup.Should().Contain(x => x.Position == 2500);
    }

    private static TrackListing CreateNewTrackListingWithPosition(int position)
    {
        return new()
        {
            Artist = "UnitTest",
            Delta = 0,
            IsRecurring = true,
            Position = position,
            PlayUtcDateAndTime = DateTime.UtcNow,
            Title = "UnitTest",
            TrackId = position,
        };
    }
}
