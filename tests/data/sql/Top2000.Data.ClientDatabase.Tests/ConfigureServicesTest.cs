using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

[assembly: Parallelize]

namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class ConfigureServicesTest
{
    [TestMethod]
    public void CanConfigureServices()
    {
        Top2000ServiceBuilder builder = new();
        
        var serviceCollection = new ServiceCollection()
            .AddTop2000(configure => builder = configure)
            .BuildServiceProvider();

        serviceCollection.GetService<Top2000AssemblyDataSource>()
            .Should().NotBeNull();
        
        serviceCollection.GetService<IUpdateClientDatabase>()
            .Should().NotBeNull();
        
        serviceCollection.GetService<ITop2000AssemblyData>()
            .Should().NotBeNull();
        
        serviceCollection.GetService<SqliteConnection>()
            .Should().NotBeNull();

        serviceCollection.GetService<Top2000ServiceBuilder>()
            .Should().Be(builder);
    }

    [TestMethod]
    public void EnablingOnlineUpdatesGivesAnOtherOption()
    {
        var serviceCollection = new ServiceCollection()
            .AddTop2000(configure =>
            {
                configure.EnableOnlineUpdates();
            })
            .BuildServiceProvider();
        
        serviceCollection.GetService<OnlineDataSource>()
            .Should().NotBeNull();
    }
}
