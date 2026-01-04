using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Data;
using Top2000.Features.Editions;
using Top2000.Features.Json.Data;
using Top2000.Features.Json.Editions;
using Top2000.Features.Json.Listings;
using Top2000.Features.Json.Searching;
using Top2000.Features.Json.TrackInformation;
using Top2000.Features.Listings;
using Top2000.Features.Searching;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.Json;

public class JsonFeatureAdapter : IFeatureAdapter 
{
    private Action<IServiceCollection>? _configureDataLoader;

    public void ConfigureDataLoader(Action<IServiceCollection> services)
    {
        _configureDataLoader = services;
    }
    
    public void AddFeatureImplementors(IServiceCollection services)
    {
        _ = _configureDataLoader ?? throw new InvalidOperationException("Data loader configuration must be provided before adding feature implementors.");
        
       _configureDataLoader.Invoke(services);

       services
           .AddTransient<JsonPartialDataInitialiser>()
           .AddSingleton<DataProvider>()
           .AddTransient<IDataInitialiser, JsonDataInitialiser>()
           .AddTransient<IEditions, EditionFeature>()
           .AddTransient<IListings, ListingFeature>()
           .AddTransient<ISearch, SearchFeature>()
           .AddTransient<ITrackInformation, TrackInformationFeature>()
           ;
    }
}