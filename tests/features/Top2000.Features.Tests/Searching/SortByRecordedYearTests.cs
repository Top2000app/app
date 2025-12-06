using Top2000.Features.Searching;

namespace Top2000.Features.Tests.Searching;

[TestClass]
public class SortByRecordedYearTests
{
    [TestMethod]
    public void SortByRecordedYearSortsTrackByRecordedYear()
    {
        var trackA = new SearchedTrack { RecordedYear = 2016 };
        var trackB = new SearchedTrack { RecordedYear = 2017 };
        var trackC = new SearchedTrack { RecordedYear = 2018 };
        var trackD = new SearchedTrack { RecordedYear = 2019 };

        var tracks = new[] { trackB, trackD, trackC, trackA };

        var actual = new SortByRecordedYear().Sort(tracks);
        var expected = new[] { trackA, trackB, trackC, trackD };

        actual.Should().NotBeEmpty()
            .And.HaveCount(4)
            .And.ContainInOrder(expected);
    }
}
