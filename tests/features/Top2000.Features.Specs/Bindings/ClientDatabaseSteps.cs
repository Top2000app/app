using AwesomeAssertions;

namespace Top2000.Features.Specs.Bindings;

[Binding]
public class ClientDatabaseSteps
{
    [BeforeScenario]
    public void DeleteClientDatabase()
    {
        if (File.Exists(App.DatabasePath))
        {
            File.Delete(App.DatabasePath);
        }
    }

    [AfterScenario]
    public void CloseDatabaseConnections()
    {
        SqliteConnection.ClearAllPools();
    }

    [Given(@"A new install of the application")]
    public void GivenANewInstallOfTheApplication()
    {
    }

    [When(@"the application starts")]
    public async Task WhenIStartTheApplication()
    {
        var assemblySource = App.GetService<Top2000AssemblyDataSource>();
        var update = App.GetService<IUpdateClientDatabase>();

        await update.RunAsync(assemblySource);
    }

    [Given(@"an installed application without the last (.*) SQL scripts")]
    public async Task GivenAnInstalledApplicationWithoutTheLastSQLScripts(int skipLast)
    {
        var assemblySource = App.GetService<Top2000AssemblyDataSource>();
        var update = App.GetService<IUpdateClientDatabase>();
        var specSourceDecorator = new Top2000AssemblyDataSourceSpecDecorator(assemblySource, skipLast);

        await update.RunAsync(specSourceDecorator);
    }

    [Then(@"the client database is created with the scripts from the top2000 data assembly")]
    public async Task ThenTheClientDatabaseIsCreatedWithTheScriptsFromTheTopDataAssembly()
    {
        var database = App.GetService<SqliteConnection>();
        var top2000AssemblyData = App.GetService<ITop2000AssemblyData>();

        var scripts = new List<string>();
        await database.OpenAsync();
        try
        {
            await using var cmd = database.CreateCommand();
            cmd.CommandText = "SELECT ScriptName FROM Journal";
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                scripts.Add(reader.GetString(0));
            }
        }
        finally
        {
            await database.CloseAsync();
        }

        var expected = top2000AssemblyData.GetAllSqlFiles()
            .ToList();

        scripts.Should().BeEquivalentTo(expected);
    }

    [When(@"the application starts without the last SQL scripts")]
    public void WhenTheApplicationStartsWithoutTheLastSQLScripts()
    {
        // nothing to do here since the client database is already
        // installed with the last SQL scripts
    }

    [Then(@"the application checks online for updates")]
    public async Task ThenTheApplicationChecksOnlineForUpdates()
    {
        var onlineSource = App.GetService<OnlineDataSource>();
        var update = App.GetService<IUpdateClientDatabase>();

        await update.RunAsync(onlineSource);
    }

    [Then(@"the application updates the second-to-last script from the assembly")]
    public async Task ThenTheApplicationUpdatesTheSecondToLastScriptFromTheAssembly()
    {
        var assemblySource = App.GetService<Top2000AssemblyDataSource>();
        var update = App.GetService<IUpdateClientDatabase>();
        var specSourceDecorator = new Top2000AssemblyDataSourceSpecDecorator(assemblySource, 1);

        await update.RunAsync(specSourceDecorator);
    }

    [Then(@"the client database is updated")]
    public async Task ThenTheClientDatabaseIsUpdated()
    {
        // since the data on the website must be the same as on the Assembly
        // we can assert here

        var database = App.GetService<SqliteConnection>();
        var top2000AssemblyData = App.GetService<ITop2000AssemblyData>();

        var scripts = new List<string>();
        await database.OpenAsync();
        try
        {
            await using var cmd = database.CreateCommand();
            cmd.CommandText = "SELECT ScriptName FROM Journal";
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                scripts.Add(reader.GetString(0));
            }
        }
        finally
        {
            await database.CloseAsync();
        }

        var expected = top2000AssemblyData.GetAllSqlFiles()
            .ToList();

        scripts.Should().BeEquivalentTo(expected);
    }
}
