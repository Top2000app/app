using Top2000.Features.TrackInformation;

namespace Top2000.Features.Tests.TrackInformation;

[TestClass]
public class ListingInformationTests
{
    [TestMethod]
    public void EditionCouldBeListedWhenYearIsEqualToRecordedDate()
    {
        var sut = new ListingInformation { Edition = 2000 };

        sut.CouldBeListed(2000).Should().BeTrue();
    }

    [TestMethod]
    public void EditionCouldBeListedWhenYearIsHigherToRecordedDate()
    {
        var sut = new ListingInformation { Edition = 2001 };

        sut.CouldBeListed(2002).Should().BeFalse();
    }

    [TestMethod]
    public void EditionsBeforeTheTrackIsRecordedCanNotBeListed()
    {
        var sut = new ListingInformation { Edition = 1999 };

        sut.CouldBeListed(2000).Should().BeFalse();
    }
}
