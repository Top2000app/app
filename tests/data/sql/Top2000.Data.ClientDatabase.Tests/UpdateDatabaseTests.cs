using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class UpdateDatabaseTests
{
    private readonly string _databaseFileName;
    private readonly SqliteConnection _connection;
    private readonly UpdateDatabase _sut;
    private readonly Mock<ISource> _sourceMock;

    public UpdateDatabaseTests()
    {
        _databaseFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".db");
        _connection = new SqliteConnection($"Data Source={_databaseFileName}");
        _sut = new UpdateDatabase(_connection);
        _sourceMock = new Mock<ISource>();
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task CanRunAsync()
    {
        var top2000Data = new Top2000Data();
        var dataSource = new Top2000AssemblyDataSource(top2000Data);
        await _sut.RunAsync(dataSource);
    }
    
    [TestMethod]
    public async Task AllFileCanBeExecuteInTheDatabaseAsync()
    {
        var top2000Data = new Top2000Data();
        var dataSource = new Top2000AssemblyDataSource(top2000Data);
        await _sut.RunAsync(dataSource);

        var totalFiles = top2000Data.GetAllSqlFiles();

        await _connection.OpenAsync(TestContext.CancellationToken);
        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT ScriptName FROM Journal";
        await using var reader = await cmd.ExecuteReaderAsync(TestContext.CancellationToken);
        var journalsCount = 0;
        while (await reader.ReadAsync(TestContext.CancellationToken))
        {
            journalsCount++;
        }
        await _connection.CloseAsync();

        journalsCount.Should().Be(totalFiles.Count);
    }

    [TestCleanup]
    public async Task TestCleanupAsync()
    {
        await _connection.CloseAsync();

        if (File.Exists(_databaseFileName))
        {
            File.Delete(_databaseFileName);
        }
    }

    [TestMethod]
    public async Task ForEveryScriptAJournalIsInsertedAsync()
    {
        _sourceMock
            .Setup(x => x.ExecutableScriptsAsync(It.IsAny<ImmutableSortedSet<string>>()))
            .ReturnsAsync(Create.ImmutableSortedSetFrom("000-First.sql"));

        _sourceMock.Setup(x => x.ScriptContentsAsync("000-First.sql")).ReturnsAsync(new SqlScript("000-First.sql", string.Empty));

        await _sut.RunAsync(_sourceMock.Object);
        _sourceMock.Verify(x => x.ScriptContentsAsync("000-First.sql"), Times.Once);

        await _connection.OpenAsync();
        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT ScriptName FROM Journal";
        await using var reader = await cmd.ExecuteReaderAsync(TestContext.CancellationToken);
        var actual = new List<string>();
        while (await reader.ReadAsync(TestContext.CancellationToken))
        {
            actual.Add(reader.GetString(0));
        }
        await _connection.CloseAsync();

        var expected = new[] { "000-First.sql" };

        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task AllSectionOfTheScriptAreExecutedAsync()
    {
        _sourceMock
            .Setup(x => x.ExecutableScriptsAsync(It.IsAny<ImmutableSortedSet<string>>()))
            .ReturnsAsync(Create.ImmutableSortedSetFrom("000-First.sql"));

        const string sql = "CREATE TABLE Table1(Id INT NOT NULL, PRIMARY KEY(Id));INSERT INTO Table1(Id) VALUES (1),(2);";
        _sourceMock
            .Setup(x => x.ScriptContentsAsync("000-First.sql"))
            .ReturnsAsync(new SqlScript("000-First.sql", sql));

        await _sut.RunAsync(_sourceMock.Object);

        await _connection.OpenAsync(TestContext.CancellationToken);
        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Table1";
        var scalar = await cmd.ExecuteScalarAsync(TestContext.CancellationToken);
        var count = (long)(scalar ?? 0L);
        await _connection.CloseAsync();

        count.Should().Be(2);
    }

    [TestMethod]
    public async Task ForFaultyScriptsJournalIsNotWrittenAsync()
    {
        _sourceMock
            .Setup(x => x.ExecutableScriptsAsync(It.IsAny<ImmutableSortedSet<string>>()))
            .ReturnsAsync(Create.ImmutableSortedSetFrom("000-First.sql", "001-Second.sql"));

        const string sql1 = "CREATE TABLE Table1(Id INT NOT NULL, PRIMARY KEY(Id));INSERT INTO Table1(Id) VALUES (1),(2);";
        const string sql2 = "INSERT INTO Table1(Id) VALUES ('2')";
        _sourceMock.Setup(x => x.ScriptContentsAsync("000-First.sql")).ReturnsAsync(new SqlScript("000-First.sql", sql1));
        _sourceMock.Setup(x => x.ScriptContentsAsync("001-Second.sql")).ReturnsAsync(new SqlScript("001-Second.sql", sql2));

        try
        {
            await _sut.RunAsync(_sourceMock.Object);
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (Exception)
        {
            // Expected
        }

        await _connection.OpenAsync(TestContext.CancellationToken);
        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT ScriptName FROM Journal";
        await using var reader = await cmd.ExecuteReaderAsync(TestContext.CancellationToken);
        var actual = new List<string>();
        while (await reader.ReadAsync(TestContext.CancellationToken))
        {
            actual.Add(reader.GetString(0));
        }
        await _connection.CloseAsync();

        var expected = new[] { "000-First.sql" };

        actual.Should().BeEquivalentTo(expected);
    }

}