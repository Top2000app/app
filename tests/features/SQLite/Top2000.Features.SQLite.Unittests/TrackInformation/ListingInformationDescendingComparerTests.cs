using Top2000.Features.SQLite.TrackInformation;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite.Unittests.TrackInformation;

[TestClass]
public class ListingInformationDescendingComparerTests
{
    [TestMethod]
    public void Listings_with_same_edition_compare_as_zero()
    {
        var listing1 = new ListingInformation { Edition = 2018 };
        var listing2 = new ListingInformation { Edition = 2018 };

        var sut = new ListingInformationDescendingComparer();

        sut.Compare(listing1, listing2).Should().Be(0);
    }

    [TestMethod]
    public void Listings_with_higher_edition_compare_as_minus_zero()
    {
        var listing1 = new ListingInformation { Edition = 2018 };
        var listing2 = new ListingInformation { Edition = 2019 };

        var sut = new ListingInformationDescendingComparer();

        sut.Compare(listing1, listing2).Should().Be(1);
    }

    [TestMethod]
    public void Edition_with_lower_year_compare_as_plus_zero()
    {
        var listing1 = new ListingInformation { Edition = 2001 };
        var listing2 = new ListingInformation { Edition = 2000 };

        var sut = new ListingInformationDescendingComparer();

        sut.Compare(listing1, listing2).Should().Be(-1);
    }
}
