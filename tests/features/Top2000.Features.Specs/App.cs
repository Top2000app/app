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
    public static readonly string DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "top2000_unittest.db");
    private static Top2000Services? _top2000Services;
    private static IServiceProvider? _serviceProvider;

    public static TType GetService<TType>() where TType : notnull => ServiceProvider().GetRequiredService<TType>();

    private static IServiceProvider ServiceProvider() => _serviceProvider ?? throw new InvalidOperationException("BeforeTestRun method has not been called yet.");

    public static Top2000Services Top2000Services() => _top2000Services ?? throw new InvalidOperationException("BeforeTestRun method has not been called yet.");


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
                }) ;
        
        _serviceProvider = services.BuildServiceProvider();
        _top2000Services = _serviceProvider.GetRequiredService<Top2000Services>();
    }
}
