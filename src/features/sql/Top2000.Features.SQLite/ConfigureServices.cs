using Microsoft.Extensions.DependencyInjection;
using Top2000.Data.ClientDatabase;
using Top2000.Features.Searching;
using Top2000.Features.SQLite.AllListingsOfEdition;

namespace Top2000.Features.SQLite;

public static class ConfigureServices
{
    public static IServiceCollection AddTop2000Features(this IServiceCollection services, Action<Top2000ServiceBuilder>? configure = null)
    {
        return services
            .AddTop2000(configure)
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<TrackCountHolder>();
            })
            .AddSingleton<TrackCountHolder>()
            .AddSingleton<ISort, SortByTitle>()
            .AddSingleton<ISort, SortByArtist>()
            .AddSingleton<ISort, SortByRecordedYear>()
            .AddSingleton<IGroup, GroupByNothing>()
            .AddSingleton<IGroup, GroupByArtist>()
            .AddSingleton<IGroup, GroupByRecordedYear>();
    }
}
