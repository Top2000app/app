using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Top2000.Features.Editions;
using Top2000.Features.Searching;

namespace Top2000.Features;

public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTop2000Features<TAdapter>(Action<TAdapter>? configure = null ) where TAdapter : IFeatureAdapter, new()
        {
            var adapter = new TAdapter();
            configure?.Invoke(adapter);
            
            adapter
                .AddFeatureImplementors(services);
            
            return services
                .AddTransient<Top2000Services>()
                .AddSingleton<ISort, SortByTitle>()
                .AddSingleton<ISort, SortByArtist>()
                .AddSingleton<ISort, SortByRecordedYear>()
                .AddSingleton<IGroup, GroupByNothing>()
                .AddSingleton<IGroup, GroupByArtist>()
                .AddSingleton<IGroup, GroupByRecordedYear>();
        }
    }
}

public interface IFeatureAdapter
{
    void AddFeatureImplementors(IServiceCollection services);
}
