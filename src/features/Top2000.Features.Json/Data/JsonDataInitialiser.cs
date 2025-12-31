using System.Text.Json;
using System.Text.Json.Serialization;
using Top2000.Data.JsonClientDatabase;
using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Data;

namespace Top2000.Features.Json.Data;

public class JsonDataInitialiser : IDataInitialiser
{
    private readonly IDataLoader _dataLoader;
    private readonly DataProvider _dataProvider;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _isInitialised = false;

    public JsonDataInitialiser(IDataLoader dataLoader, DataProvider dataProvider)
    {
        _dataLoader = dataLoader;
        _dataProvider = dataProvider;

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = new ShortNameNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task InitialiseDataAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialised)
        {
            return;
        }

        _isInitialised = true;
        
        var dataStream = await _dataLoader.LoadDataAsync(cancellationToken);

        var data = await JsonSerializer.DeserializeAsync<Top2000File>(dataStream, _jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize JSON data into Top2000DataContext.");

        _dataProvider.SetValue(new Top2000DataContext
        {
            Version = data.Version,
            Listings = data.Listings,
            ListingsOfEdition = data.Listings
                .GroupBy(l => l.TrackId)
                .ToDictionary(g => g.Key, g => g.ToList()),
            Tracks = data.Tracks.ToDictionary(x => x.Id),
            Editions = data.Editions.ToList()
        });
    }

    public Task<int> DataVersion(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dataProvider.Value.Version);
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        _isInitialised = false;
        return InitialiseDataAsync(cancellationToken);
    }
}