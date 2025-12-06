namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class Top2000ServiceBuilderTests
{
    private readonly Top2000ServiceBuilder _sut = new();

    [TestMethod]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        _sut.Directory.Should().Be(AppDomain.CurrentDomain.BaseDirectory);
        _sut.Name.Should().Be("Top2000v2.db");
        _sut.OnlineUpdatesEnabled.Should().BeTrue();
        _sut.UpdateUri.Should().Be(new Uri("https://data.top2000.app/"));
    }

    [TestMethod]
    public void DatabaseDirectory_ShouldSetCorrectly()
    {
        const string customDirectory = "/custom/directory";

        _sut.DatabaseDirectory(customDirectory);

        _sut.Directory.Should().Be(customDirectory);
    }

    [TestMethod]
    public void DatabaseName_ShouldSetCorrectly()
    {
        const string customName = "CustomDatabase.db";

        _sut.DatabaseName(customName);

        _sut.Name.Should().Be(customName);
    }

    [TestMethod]
    public void EnableOnlineUpdates_ShouldEnableFeature()
    {
        _sut.EnableOnlineUpdates();

        _sut.OnlineUpdatesEnabled.Should().BeTrue();
    }

    [TestMethod]
    public void EnableOnlineUpdates_ShouldDisableFeature()
    {
        _sut.EnableOnlineUpdates(false);

        _sut.OnlineUpdatesEnabled.Should().BeFalse();
    }

    [TestMethod]
    public void EnableOnlineUpdates_WithCustomUri_ShouldSetCorrectly()
    {
        var customUri = new Uri("https://custom.uri/");

        _sut.EnableOnlineUpdates(customUri);

        _sut.UpdateUri.Should().Be(customUri);
        _sut.OnlineUpdatesEnabled.Should().BeTrue();
    }
}
