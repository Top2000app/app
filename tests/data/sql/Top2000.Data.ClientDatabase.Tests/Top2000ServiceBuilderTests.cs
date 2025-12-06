namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class Top2000ServiceBuilderTests
{
    private readonly Top2000ServiceBuilder _sut = new();

    [TestMethod]
    public void DefaultValuesShouldBeSetCorrectly()
    {
        _sut.Directory.Should().Be(AppDomain.CurrentDomain.BaseDirectory);
        _sut.Name.Should().Be("Top2000v2.db");
        _sut.OnlineUpdatesEnabled.Should().BeTrue();
        _sut.UpdateUri.Should().Be(new Uri("https://data.top2000.app/"));
    }

    [TestMethod]
    public void DatabaseDirectoryShouldSetCorrectly()
    {
        const string customDirectory = "/custom/directory";

        _sut.DatabaseDirectory(customDirectory);

        _sut.Directory.Should().Be(customDirectory);
    }

    [TestMethod]
    public void DatabaseNameShouldSetCorrectly()
    {
        const string customName = "CustomDatabase.db";

        _sut.DatabaseName(customName);

        _sut.Name.Should().Be(customName);
    }

    [TestMethod]
    public void EnableOnlineUpdatesShouldEnableFeature()
    {
        _sut.EnableOnlineUpdates();

        _sut.OnlineUpdatesEnabled.Should().BeTrue();
    }

    [TestMethod]
    public void EnableOnlineUpdatesShouldDisableFeature()
    {
        _sut.EnableOnlineUpdates(false);

        _sut.OnlineUpdatesEnabled.Should().BeFalse();
    }

    [TestMethod]
    public void EnableOnlineUpdatesWithCustomUriShouldSetCorrectly()
    {
        var customUri = new Uri("https://custom.uri/");

        _sut.EnableOnlineUpdates(customUri);

        _sut.UpdateUri.Should().Be(customUri);
        _sut.OnlineUpdatesEnabled.Should().BeTrue();
    }
}
