using Top2000.Features.AllEditions;
using Top2000.Features.SQLite.AllEditions;

namespace Top2000.Features.SQLite.Unittests.AllEditions;

[TestClass]
public class EditionComparerTests
{
    private readonly EditionDescendingComparer sut;

    public EditionComparerTests()
    {
        sut = new EditionDescendingComparer();
    }

    [TestMethod]
    public void EditionsWithSameYearCompareAsZero()
    {
        var edition1 = new Edition { Year = 2018 };
        var edition2 = new Edition { Year = 2018 };

        sut.Compare(edition1, edition2).ShouldBe(0);
    }

    [TestMethod]
    public void EditionWithHigherYearCompareAsMinusZero()
    {
        var edition1 = new Edition { Year = 2018 };
        var edition2 = new Edition { Year = 2019 };

        sut.Compare(edition1, edition2).ShouldBe(1);
    }

    [TestMethod]
    public void EditionWithLowerYearCompareAsPlusZero()
    {
        var edition1 = new Edition { Year = 2001 };
        var edition2 = new Edition { Year = 2000 };

        sut.Compare(edition1, edition2).ShouldBe(-1);
    }
}
