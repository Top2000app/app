﻿using Top2000.Features.AllEditions;
using Top2000.Features.SQLite.AllEditions;

namespace Top2000.Features.SQLite.Unittests.AllEditions;

[TestClass]
public class EditionDescendingComparerTests
{
    private readonly EditionDescendingComparer sut;

    public EditionDescendingComparerTests()
    {
        sut = new EditionDescendingComparer();
    }

    [TestMethod]
    public void Editions_with_same_year_compare_as_zero()
    {
        var edition1 = new Edition { Year = 2018 };
        var edition2 = new Edition { Year = 2018 };

        sut.Compare(edition1, edition2).ShouldBe(0);
    }

    [TestMethod]
    public void Edition_with_higher_year_compare_as_minus_zero()
    {
        var edition1 = new Edition { Year = 2018 };
        var edition2 = new Edition { Year = 2019 };

        sut.Compare(edition1, edition2).ShouldBe(1);
    }

    [TestMethod]
    public void Edition_with_lower_year_compare_as_plus_zero()
    {
        var edition1 = new Edition { Year = 2001 };
        var edition2 = new Edition { Year = 2000 };

        sut.Compare(edition1, edition2).ShouldBe(-1);
    }
}
