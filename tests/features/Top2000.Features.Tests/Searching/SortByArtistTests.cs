using Top2000.Features.Searching;

namespace Top2000.Features.Tests.Searching;

[TestClass]
public class SortByArtistTests
{
    [TestMethod]
    public void SortByArtistSortTrackByTheNameOfTheArtistAlphabetically()
    {
        var trackA = new SearchedTrack { Artist = "A" };
        var trackB = new SearchedTrack { Artist = "B" };
        var trackC = new SearchedTrack { Artist = "C" };
        var trackD = new SearchedTrack { Artist = "D" };

        var tracks = new[] { trackB, trackD, trackC, trackA };

        var actual = new SortByArtist().Sort(tracks);
        var expected = new[] { trackA, trackB, trackC, trackD };

        actual.Should().NotBeEmpty()
            .And.HaveCount(4)
            .And.ContainInOrder(expected);
    }
}
