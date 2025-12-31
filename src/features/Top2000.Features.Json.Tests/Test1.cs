using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Top2000.Features.Json.Tests;


[TestClass]
public sealed class Test1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var top2000 = new ServiceCollection()
            .AddTop2000Features<JsonFeatureAdapter>(configure =>
            {
                configure.ConfigureDataLoader(services =>
                {
                    services.AddSingleton<IDataLoader, DataLoader>();
                });
            })
            .BuildServiceProvider()
            .GetRequiredService<Top2000Services>();

        await top2000.InitialiseDataAsync();
        var editions = await top2000.AllListingsOfEditionAsync(2025);
        
        Assert.IsNotNull(editions);
        Assert.IsTrue(editions.Any());
    }
}

public class DataLoader : IDataLoader
{
    public Task<Stream> LoadDataAsync(CancellationToken cancellationToken = default)
    {
        var stream = File.OpenText("top2000.json");
        return Task.FromResult(stream.BaseStream);
    }
}