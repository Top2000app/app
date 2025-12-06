using Top2000.Features.TrackInformation;

namespace Top2000.Features.Tests.TrackInformation;

[TestClass]
public class ListingInformationTests
{
    [TestMethod]
    public void Edition_could_be_listed_when_year_is_equal_to_recorded_date()
    {
        var sut = new ListingInformation { Edition = 2000 };

        sut.CouldBeListed(2000).Should().BeTrue();
    }

    [TestMethod]
    public void Edition_could_be_listed_when_year_is_higher_to_recorded_date()
    {
        var sut = new ListingInformation { Edition = 2001 };

        sut.CouldBeListed(2002).Should().BeFalse();
    }

    [TestMethod]
    public void Editions_before_the_track_is_recorded_can_not_be_listed()
    {
        var sut = new ListingInformation { Edition = 1999 };

        sut.CouldBeListed(2000).Should().BeFalse();
    }
}
