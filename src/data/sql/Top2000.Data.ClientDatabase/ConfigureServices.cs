using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Top2000.Data.ClientDatabase;

public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTop2000ClientDatabase(
            ConfigurationManager configurationManager,
            Action<Top2000ServiceBuilder>? configure = null)
        {
            var builder = new Top2000ServiceBuilder();
            configure?.Invoke(builder);
            var connectionString = $"Data Source={Path.Combine(builder.Directory, builder.Name)}";

            configurationManager["ConnectionStrings:Top2000"] = connectionString;

            services
                .Configure<Top2000DataOptions>(configurationManager.GetRequiredSection("ConnectionStrings:Top2000"))
                .AddTransient<Top2000AssemblyDataSource>()
                .AddTransient<IUpdateClientDatabase, UpdateDatabase>()
                .AddTransient<ITop2000AssemblyData, Top2000Data>()
                .AddTransient<SqliteConnection>(x => new SqliteConnection(connectionString))
                .AddSingleton(builder);

            if (builder.OnlineUpdatesEnabled)
            {
                services
                    .AddHttpClient<OnlineDataSource>(client => { client.BaseAddress = builder.UpdateUri; });
            }

            return services;
        }
    }
}
