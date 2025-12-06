using Top2000.Features.TrackInformation;
using Top2000.Features.SQLite.TrackInformation;

namespace Top2000.Features.SQLite.Unittests.TrackInformation;

[TestClass]
public class ListingStatusStrategyTests
{
    [TestMethod]
    public void NotAvailableWhenListingInformationIsNull()
    {
        var sut = new ListingStatusStrategy();
        var listing = (ListingInformation?)null;

        sut.Determine(listing).Should().Be(ListingStatus.NotAvailable);
    }

    [TestMethod]
    public void NotListedWhenListingInformationIsNotSet()
    {
        var sut = new ListingStatusStrategy();
        var listing = new ListingInformation();

        sut.Determine(listing).Should().Be(ListingStatus.NotListed);
    }

    [TestMethod]
    public void NewWhenListingInformationIsNew()
    {
        var sut = new ListingStatusStrategy();
        var listing = new ListingInformation { Edition = 2023, PreviousEdition = 0 };

        sut.Determine(listing).Should().Be(ListingStatus.New);
    }

    [TestMethod]
    public void BackWhenListingInformationBack()
    {
        var sut = new ListingStatusStrategy();
        var listing = new ListingInformation { Edition = 2023, PreviousEdition = 2012, PreviousPosition = 100 };

        sut.Determine(listing).Should().Be(ListingStatus.Back);
    }

    [TestMethod]
    public void UnchangedWhenPositionIsTheSame()
    {
        var sut = new ListingStatusStrategy();
        var current = new ListingInformation { Position = 100, PreviousPosition = 100 };

        sut.Determine(current).Should().Be(ListingStatus.Unchanged);
    }

    [TestMethod]
    public void IncreasedWhenPositionIsLower()
    {
        var sut = new ListingStatusStrategy();
        var current = new ListingInformation { Position = 10, PreviousPosition = 100 };

        sut.Determine(current).Should().Be(ListingStatus.Increased);
    }

    [TestMethod]
    public void DecreasedWhenPositionIsHigher()
    {
        var sut = new ListingStatusStrategy();
        var current = new ListingInformation { Position = 100, PreviousPosition = 10 };

        sut.Determine(current).Should().Be(ListingStatus.Decreased);
    }
}
