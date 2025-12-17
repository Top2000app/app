using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Top2000.Data.ClientDatabase;
using Top2000.Features.Editions;
using Top2000.Features.Listing;
using Top2000.Features.Searching;
using Top2000.Features.SQLite.Editions;
using Top2000.Features.SQLite.Listings;
using Top2000.Features.SQLite.Searching;
using Top2000.Features.SQLite.TrackInformation;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite;

public class SqliteFeatureAdapter : IFeatureAdapter
{
    private readonly Action<Top2000ServiceBuilder>? _configure;

    public SqliteFeatureAdapter(Action<Top2000ServiceBuilder>? configure = null)
    {
        _configure = configure;
    }
    
    public void AddFeatureImplementors(ConfigurationManager configurationManager, IServiceCollection services)
    {
        services
            .AddSingleton<TrackCountHolder>()
            .AddTop2000ClientDatabase(configurationManager, _configure)
            .AddTransient<IEditions, EditionFeature>()
            .AddTransient<IListings, ListingFeature>()
            .AddTransient<ISearch, SearchFeature>()
            .AddTransient<ITrackInformation, TrackInformationFeature>()
            ;
    }
}