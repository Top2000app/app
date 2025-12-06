using Microsoft.Extensions.DependencyInjection;

namespace Top2000.Data.ClientDatabase;

public static class ConfigureServices
{
    public static IServiceCollection AddTop2000(this IServiceCollection services,
        Action<Top2000ServiceBuilder>? configure = null)
    {
        var builder = new Top2000ServiceBuilder();

        configure?.Invoke(builder);

        services
            .AddTransient<Top2000AssemblyDataSource>()
            .AddTransient<IUpdateClientDatabase, UpdateDatabase>()
            .AddTransient<ITop2000AssemblyData, Top2000Data>()
            .AddSingleton(builder);

        services.AddTransient<SqliteConnection>(provider =>
        {
            var top2000Builder = provider.GetRequiredService<Top2000ServiceBuilder>();
            var databasePath = Path.Combine(top2000Builder.Directory, top2000Builder.Name);
            var connectionString = $"Data Source={databasePath}";

            return new SqliteConnection(connectionString);
        });

        if (builder.OnlineUpdatesEnabled)
        {
            services
                .AddTransient<OnlineDataSource>()
                .AddHttpClient("top2000", client => { client.BaseAddress = builder.UpdateUri; });
        }

        return services;
    }
}
