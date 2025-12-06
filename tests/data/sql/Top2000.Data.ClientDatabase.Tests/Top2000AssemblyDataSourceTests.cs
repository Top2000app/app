namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class Top2000AssemblyDataSourceTests
{
    private readonly Mock<ITop2000AssemblyData> _dataMock;
    private readonly Top2000AssemblyDataSource _sut;

    public Top2000AssemblyDataSourceTests()
    {
        _dataMock = new Mock<ITop2000AssemblyData>();
        _sut = new Top2000AssemblyDataSource(_dataMock.Object);
    }

    [TestMethod]
    public async Task ExecutableScriptsAreComingFromTheDataAssemblyAsync()
    {
        var set = new HashSet<string>()
        {
            "001-Script1.sql",
            "002-Script2.sql"
        };
        _dataMock.Setup(x => x.GetAllSqlFiles()).Returns(set);

        var scripts = await _sut.ExecutableScriptsAsync([]);

        scripts.Should().BeEquivalentTo(set);
    }

    [TestMethod]
    public async Task ExecutableScriptDoesNotIncludeScriptAlreadyInJournalAsync()
    {
        var set = new HashSet<string>()
        {
            "001-Script1.sql",
            "002-Script2.sql"
        };
        _dataMock.Setup(x => x.GetAllSqlFiles()).Returns(set);

        var journals = Create.ImmutableSortedSetFrom("001-Script1.sql");

        var scripts = await _sut.ExecutableScriptsAsync(journals);

        var expected = Create.ImmutableSortedSetFrom("002-Script2.sql");

        scripts.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task ScriptContentsIsContentFromTheDataAssemblyTransformedInASqlScriptClassAsync()
    {
        const string scriptName = "001-SqlScript.sql";
        const string contents = "CREATE TABLE(id INT);";
        _dataMock.Setup(x => x.GetScriptContentAsync(scriptName)).ReturnsAsync(contents);

        var sqlScript = await _sut.ScriptContentsAsync(scriptName);

        sqlScript.ScriptName.Should().Be(scriptName);
        sqlScript.Contents.Should().Be(contents);
    }
}
