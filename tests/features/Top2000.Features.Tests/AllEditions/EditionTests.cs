using Top2000.Features.Editions;

[assembly: Parallelize]

namespace Top2000.Features.Tests.AllEditions;

[TestClass]
public class EditionTests
{
    private readonly Edition _sut = new()
    {
        Year = 2023,
        StartUtcDateAndTime = DateTime.UtcNow,
        EndUtcDateAndTime = DateTime.UtcNow,
        HasPlayDateAndTime = true
    };

    [TestMethod]
    public void LocalStartDateAndTimeTransformsTheUtc()
    {
        _sut.LocalStartDateAndTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void LocalEndDateAndTimeTransformsFromUtc()
    {
        _sut.LocalEndDateAndTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }
}
