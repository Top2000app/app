using Top2000.Features.Searching;

namespace Top2000.Features.Tests.Searching;

[TestClass]
public class GroupByRecordedYearTests
{
    [TestMethod]
    public void GroupByRecordedYearGroupsTrackOnTheRecordedYear()
    {
        var track2016_1 = new SearchedTrack { RecordedYear = 2016 };
        var track2016_2 = new SearchedTrack { RecordedYear = 2016 };
        var track2017_1 = new SearchedTrack { RecordedYear = 2017 };
        var track2018_1 = new SearchedTrack { RecordedYear = 2018 };
        var track2018_2 = new SearchedTrack { RecordedYear = 2018 };

        var tracks = new[] { track2018_2, track2016_1, track2016_2, track2017_1, track2018_1 };
        var actual = new GroupByRecordedYear().Group(tracks);

        actual.Should().NotBeEmpty()
            .And.HaveCount(3);

        actual.First().Key.Should().Be("2018");
        actual.First().Should().NotBeEmpty()
            .And.HaveCount(2)
            .And.ContainInOrder(track2018_2, track2018_1);

        actual.Skip(1).First().Key.Should().Be("2016");
        actual.Skip(1).First().Should().NotBeEmpty()
            .And.HaveCount(2)
            .And.ContainInOrder(track2016_1, track2016_2);

        actual.Skip(2).First().Key.Should().Be("2017");
        actual.Skip(2).First().Should().NotBeEmpty()
            .And.HaveCount(1)
            .And.ContainInOrder(track2017_1);
    }
}
