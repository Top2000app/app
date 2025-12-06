using Top2000.Features.SQLite.TrackInformation;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite.Unittests.TrackInformation;

[TestClass]
public class ListingStatusStrategyTests
{
    private readonly ListingStatusStrategy sut;

    public ListingStatusStrategyTests()
    {
        sut = new ListingStatusStrategy(2002);
    }

    [TestMethod]
    public void Status_is_NotAvailable_when_the_track_is_not_recorded()
    {
        var listing = new ListingInformation { Edition = 2000 };

        sut.Determine(listing).ShouldBe(ListingStatus.NotAvailable);
    }

    [TestMethod]
    public void Status_is_NotListed_when_track_has_recorded_but_position_is_null()
    {
        var listing = new ListingInformation { Edition = 2002 };

        sut.Determine(listing).ShouldBe(ListingStatus.NotListed);
    }

    [TestMethod]
    public void Status_is_New_when_track_has_position_is_filled_and_not_been_set()
    {
        var listing = new ListingInformation { Edition = 2002, Position = 12 };

        sut.Determine(listing).ShouldBe(ListingStatus.New);
    }

    [TestMethod]
    public void Status_is_Back_when_previous_status_is_NotListed_and_new_has_been_set()
    {
        new List<ListingInformation>
        {
            new() { Edition = 1999, Status = ListingStatus.NotAvailable},
            new() { Edition = 2000, Status = ListingStatus.NotAvailable},
            new() { Edition = 2001, Status = ListingStatus.NotAvailable},
            new() { Edition = 2002, Position = 1, Status = ListingStatus.New},
            new() { Edition = 2003, Status = ListingStatus.NotListed},
        }
        .ForEach(x => sut.Determine(x));

        var listing = new ListingInformation { Edition = 2004, Position = 12 };

        sut.Determine(listing).ShouldBe(ListingStatus.Back);
    }

    [TestMethod]
    public void Status_is_Unchanged_when_offset_0()
    {
        new List<ListingInformation>
        {
            new() { Edition = 1999, Status = ListingStatus.NotAvailable},
            new() { Edition = 2000, Status = ListingStatus.NotAvailable},
            new() { Edition = 2001, Status = ListingStatus.NotAvailable},
            new() { Edition = 2002, Position = 2, Status = ListingStatus.New},
        }
        .ForEach(x => sut.Determine(x));

        var current = new ListingInformation { Edition = 2003, Position = 2, Offset = 0 };
        sut.Determine(current).ShouldBe(ListingStatus.Unchanged);
    }

    [TestMethod]
    public void Status_is_Increased_when_offset_negative()
    {
        new List<ListingInformation>
        {
            new() { Edition = 1999, Status = ListingStatus.NotAvailable},
            new() { Edition = 2000, Status = ListingStatus.NotAvailable},
            new() { Edition = 2001, Status = ListingStatus.NotAvailable},
            new() { Edition = 2002, Position = 2, Status = ListingStatus.New},
        }
      .ForEach(x => sut.Determine(x));

        var current = new ListingInformation { Edition = 2003, Position = 1, Offset = -1 };
        sut.Determine(current).ShouldBe(ListingStatus.Increased);
    }

    [TestMethod]
    public void Status_is_Decreased_when_offset_positive()
    {
        new List<ListingInformation>
        {
            new() { Edition = 1999, Status = ListingStatus.NotAvailable},
            new() { Edition = 2000, Status = ListingStatus.NotAvailable},
            new() { Edition = 2001, Status = ListingStatus.NotAvailable},
            new() { Edition = 2002, Position = 2, Status = ListingStatus.New},
        }
      .ForEach(x => sut.Determine(x));

        var current = new ListingInformation { Edition = 2003, Position = 3, Offset = 1 };
        sut.Determine(current).ShouldBe(ListingStatus.Decreased);
    }
}
