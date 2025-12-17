namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class OnlineDataSourceTests
{
    private readonly Mock<HttpMessageHandler> _messageMock = new();

    [TestMethod]
    public async Task WithoutJournalsThisOnlineSourceShouldNotBeUsedThusNoExecutableScriptAreReturned()
    {
        var noJournals = ImmutableSortedSet<string>.Empty;

        var sut = new OnlineDataSource(httpClient: new HttpClient(_messageMock.Object));
        var scripts = await sut.ExecutableScriptsAsync(noJournals);

        scripts.Should().BeEmpty();
    }

    [TestMethod]
    public async Task UponErrorExecutableScriptsIsEmptyAsync()
    {
        var journals = Create.ImmutableSortedSetFrom("001-Script.sql");

        using var response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.BadRequest;
        response.Content = new StringContent("ERROR");

        using var httpClient = SetupMocksWithResponse(response);
        var sut = new OnlineDataSource(httpClient);
        
        var scripts = await sut.ExecutableScriptsAsync(journals);

        scripts.Should().BeEmpty();
    }

    [TestMethod]
    public async Task TheLastJournalIsUsedToCallTop2000ApiAsync()
    {
        var journals = Create.ImmutableSortedSetFrom("001-Script.sql", "002-Script.sql");
        var upgrades = new[] { "003-Script3.sql" };
        var content = JsonSerializer.Serialize(upgrades);

        using var response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.OK;
        response.Content = new StringContent(content);
        
        using var httpClient = SetupMocksWithResponse(response);
        var sut = new OnlineDataSource(httpClient);

        var scripts = await sut.ExecutableScriptsAsync(journals);

        var expectedUri = new Uri("http://unittest:2000/versions/002/upgrades");

        _messageMock.Protected()
            .Verify("SendAsync", Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri == expectedUri),
            ItExpr.IsAny<CancellationToken>());

        scripts.Should().BeEquivalentTo(upgrades);
    }

    [TestMethod]
    public async Task ScriptIsRetrievedFromDataEndpointAsync()
    {
        using var response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.OK;
        response.Content = new StringContent("CREATE TABLE table(Id INT NOT NULL);");
        
        using var httpClient = SetupMocksWithResponse(response);
        var sut = new OnlineDataSource(httpClient);
        var script = await sut.ScriptContentsAsync("002-Script2.sql");

        var expectedUri = new Uri("http://unittest:2000/sql/002-Script2.sql");

        _messageMock.Protected()
           .Verify("SendAsync", Times.Once(),
           ItExpr.Is<HttpRequestMessage>(req =>
               req.Method == HttpMethod.Get &&
               req.RequestUri == expectedUri),
           ItExpr.IsAny<CancellationToken>());

        using (new AssertionScope())
        {
            script.ScriptName.Should().Be("002-Script2.sql");
            script.Contents.Should().Be("CREATE TABLE table(Id INT NOT NULL);");
        }
    }

    private HttpClient SetupMocksWithResponse(HttpResponseMessage response)
    {
        _messageMock.Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(response)
           .Verifiable();

        return new HttpClient(_messageMock.Object)
        {
            BaseAddress = new Uri("http://unittest:2000/")
        };
    }
}