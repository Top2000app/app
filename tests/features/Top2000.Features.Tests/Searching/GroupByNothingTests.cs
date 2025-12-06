using Top2000.Features.Searching;

namespace Top2000.Features.Tests.Searching;

[TestClass]
public class GroupByNothingTests
{
    [TestMethod]
    public void GroupByNothingPutsAllTrackIntoOneGroupWithTheCountAsKey()
    {
        var trackA_1 = new SearchedTrack
        {
            Artist = "A",
            Id = 0,
            Title = "A",
            RecordedYear = 0,
            LatestEdition = 0
        };
        var trackB = new SearchedTrack
        {
            Artist = "B",
            Id = 0,
            Title = "A",
            RecordedYear = 0,
            LatestEdition = 0
        };
        var trackC_1 = new SearchedTrack
        {
            Artist = "C",
            Id = 0,
            Title =  "C",
            RecordedYear = 0,
            LatestEdition = 0
        };
        var trackD_1 = new SearchedTrack
        {
            Artist = "D",
            Id = 0,
            Title = "D",
            RecordedYear = 0,
            LatestEdition = 0
        };


        var tracks = new[] { trackB, trackD_1, trackC_1, trackA_1 };
        var actual = new GroupByNothing().Group(tracks);

        actual.Should().NotBeEmpty()
            .And.HaveCount(1);

        actual.First().Key.Should().Be("4");
        actual.First().Should().NotBeEmpty()
            .And.HaveCount(4)
            .And.ContainInOrder(tracks);
    }
}
