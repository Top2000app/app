using Top2000.Features.Searching;

namespace Top2000.Features.Tests.Searching;

[TestClass]
public class SortByArtistTests
{
    [TestMethod]
    public void SortByArtistSortTrackByTheNameOfTheArtistAlphabetically()
    {
        var trackA = new SearchedTrack { Title = "A", Artist ="A", Id = 1, LatestEdition = 1, RecordedYear = 2016 };
        var trackB = new SearchedTrack { Title = "B", Artist ="B", Id = 1, LatestEdition = 1, RecordedYear = 2016 };
        var trackC = new SearchedTrack { Title = "C", Artist ="C", Id = 1, LatestEdition = 1, RecordedYear = 2017 };
        var trackD = new SearchedTrack { Title = "D", Artist ="D", Id = 1, LatestEdition = 1, RecordedYear = 2018 };

        var tracks = new[] { trackB, trackD, trackC, trackA };

        var actual = new SortByArtist().Sort(tracks);
        var expected = new[] { trackA, trackB, trackC, trackD };

        actual.Should().NotBeEmpty()
            .And.HaveCount(4)
            .And.ContainInOrder(expected);
    }
}
