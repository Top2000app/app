using Top2000.Features.AllEditions;

namespace Top2000.Features.Tests.AllEditions;

[TestClass]
public class EditionTests
{
    private readonly Edition sut;

    public EditionTests()
    {
        sut = new Edition();
    }

    [TestMethod]
    public void LocalStartDateAndTimeTransformsTheUtc()
    {
        sut.StartUtcDateAndTime = DateTime.UtcNow;
        sut.LocalStartDateAndTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void LocalEndDateAndTimeTransformsFromUtc()
    {
        sut.EndUtcDateAndTime = DateTime.UtcNow;
        sut.LocalEndDateAndTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }
}
