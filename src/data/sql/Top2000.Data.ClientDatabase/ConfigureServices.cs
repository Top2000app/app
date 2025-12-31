using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Top2000.Data.ClientDatabase;

public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTop2000ClientDatabase(
            Action<Top2000ServiceBuilder>? configure = null)
        {
            var builder = new Top2000ServiceBuilder();
            configure?.Invoke(builder);

            services
                .AddSingleton<Top2000ServiceBuilder>()
                .AddTransient<Top2000AssemblyDataSource>()
                .AddTransient<IUpdateClientDatabase, UpdateDatabase>()
                .AddTransient<ITop2000AssemblyData, Top2000Data>()
                .AddTransient<SqliteConnection>(x => new SqliteConnection(x.GetRequiredService<Top2000ServiceBuilder>().ConnectionString))
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
