using Microsoft.Extensions.DependencyInjection;
using Top2000.Data.StaticApiGenerator;

var services = new ServiceCollection()
    .AddLogging(configure => configure.AddConsole())
    .AddSingleton<ITop2000AssemblyData, Top2000Data>()
    .AddSingleton<ITransformSqlFiles, SqlFileTransformer>()
    .AddSingleton<IFileCreator, FileCreator>()
    .AddSingleton<IRunApplication, PublishOnlyApplication>()
    .BuildServiceProvider()
    ;

var application = services.GetRequiredService<IRunApplication>();
await application.RunAsync().ConfigureAwait(false);