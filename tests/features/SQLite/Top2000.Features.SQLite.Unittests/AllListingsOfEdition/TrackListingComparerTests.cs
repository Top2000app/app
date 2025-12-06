using Top2000.Features.AllListingsOfEdition;
using Top2000.Features.SQLite.AllListingsOfEdition;

namespace Top2000.Features.SQLite.Unittests.AllListingsOfEdition;

[TestClass]
public class TrackListingComparerTests
{
    private readonly TrackListingComparer sut;

    public TrackListingComparerTests()
    {
        sut = new TrackListingComparer();
    }

    [TestMethod]
    public void TrackListingAreEqualWhenPositionIsTheSame()
    {
        var track1 = new TrackListing { Position = 1 };
        var track2 = new TrackListing { Position = 1 };

        sut.Equals(track1, track2).ShouldBeTrue();
    }

    [TestMethod]
    public void HashCodeOfTrackListingEqualsPositionHashCode()
    {
        var track = new TrackListing { Position = 1392 };

        sut.GetHashCode(track).ShouldBe(track.Position.GetHashCode());
    }
}
