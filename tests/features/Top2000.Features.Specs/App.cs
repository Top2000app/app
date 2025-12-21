using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Top2000.Data.ClientDatabase;
using Top2000.Features.SQLite;

[assembly: TestCategory("SkipWhenLiveUnitTesting")]
[assembly: DoNotParallelize]

namespace Top2000.Features.Specs;

[Binding]
public static class App
{
    public static string DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "top2000_unittest.db");

    public static TType GetService<TType>() where TType : notnull => ServiceProvider.GetRequiredService<TType>();
    private static IServiceProvider ServiceProvider { get; set; } = new HostBuilder().Build().Services;

    public static Top2000Services Top2000Services { get; } = ServiceProvider.GetRequiredService<Top2000Services>();
    
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        var services = new ServiceCollection()
                .AddTop2000Features<SqliteFeatureAdapter>(configure =>
                {
                    configure.ConfigureClientDatabase(clientDb =>
                    {
                        clientDb
                            .DatabaseDirectory(Directory.GetCurrentDirectory())
                            .DatabaseName("top2000_unittest.db")
                            .EnableOnlineUpdates();
                    });
                })
            ;
        

        ServiceProvider = services.BuildServiceProvider();
    }
}
