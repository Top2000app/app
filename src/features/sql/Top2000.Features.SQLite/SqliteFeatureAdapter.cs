using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Top2000.Data.ClientDatabase;
using Top2000.Features.Data;
using Top2000.Features.Editions;
using Top2000.Features.Listings;
using Top2000.Features.Searching;
using Top2000.Features.SQLite.Data;
using Top2000.Features.SQLite.Editions;
using Top2000.Features.SQLite.Listings;
using Top2000.Features.SQLite.Searching;
using Top2000.Features.SQLite.TrackInformation;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite;

public class SqliteFeatureAdapter : IFeatureAdapter
{
    private Action<Top2000ServiceBuilder>? _configure = null;
    private ConfigurationManager _configurationManager = new ConfigurationManager();

    public void ConfigureClientDatabase(Action<Top2000ServiceBuilder>? configure)
    {
        _configure = configure;
    }
    
    public void OverwriteBuildInConfigurationManager(ConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }
    
    public void AddFeatureImplementors(IServiceCollection services)
    {
        services
            .AddTop2000ClientDatabase(_configurationManager, _configure)
            .AddSingleton<TrackCountHolder>()
            .AddTransient<IEditions, EditionFeature>()
            .AddTransient<IListings, ListingFeature>()
            .AddTransient<ISearch, SearchFeature>()
            .AddTransient<ITrackInformation, TrackInformationFeature>()
            .AddTransient<IDataInitialiser, SqliteDataInitialiser>()
            ;
    }
}