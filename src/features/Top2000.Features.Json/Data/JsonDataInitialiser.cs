using System.Text.Json;
using System.Text.Json.Serialization;
using Top2000.Data.JsonClientDatabase;
using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Data;

namespace Top2000.Features.Json.Data;

public class JsonPartialDataInitialiser
{
    private readonly IDataLoader _dataLoader;
    private readonly DataProvider _dataProvider;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public JsonPartialDataInitialiser(IDataLoader dataLoader, DataProvider dataProvider)
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

    public async Task InitialiseVersionDataAsync(CancellationToken cancellationToken = default)
    {
        await using var versionInfo = await _dataLoader.LoadDataVersionAsync();
        
        var top2000VersionInfo =
            await JsonSerializer.DeserializeAsync<Top2000VersionInfo>(versionInfo, _jsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize JSON data into Top2000DataContext.");

        _dataProvider.Value.Editions = top2000VersionInfo.Editions;
        _dataProvider.Value.Version = top2000VersionInfo.Version;
    }
    
    public async Task InitialiseRestAsync(CancellationToken cancellationToken = default)
    {
        if (_dataProvider.Value.Version == 0)
        {
            throw new InvalidOperationException("Editions are not initialised.");
        }

        var editionToDo = _dataProvider.Value.Editions
            .Where(edition => !_dataProvider.Value.Editions.Contains(edition))
            .ToList();
        
        foreach (var edition in editionToDo)
        {
            await InitialiseEditionAsync(edition.Year, cancellationToken);
        }
    }

    public async Task InitialiseEditionAsync(int edition, CancellationToken cancellationToken = default)
    {
        if (_dataProvider.Value.Version == 0)
        {
            throw new InvalidOperationException("Editions are not initialised.");
        }

        if (_dataProvider.Value.Editions.All(x => x.Year != edition))
        {
            throw new InvalidOperationException("Requested edition not found in data source.");
        }
        
        await using var dataStream = await _dataLoader.LoadEditionDataAsync(edition);
        var data = await JsonSerializer.DeserializeAsync<Top2000File>(dataStream, _jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize JSON data into Top2000DataContext.");
        
        _dataProvider.Value.Listings.AddRange(data.Listings);
        _dataProvider.Value.ListingsOfEdition.Add(edition, data.Listings);

        data.Listings
            .Where(x => !_dataProvider.Value.Tracks.ContainsKey(x.TrackId))
            .Select(x => new Track
            {
                Id = x.TrackId,
                Artist = x.Artist,
                Title = x.Title,
                RecordedYear = x.RecordedYear,
                SearchArtist = x.SearchArtist,
                SearchTitle = x.SearchTitle,
            })
            .ToList()
            .ForEach(x => _dataProvider.Value.Tracks.Add(x.Id, x));
    }
}

public class JsonDataInitialiser : IDataInitialiser
{
    private readonly IDataLoader _dataLoader;
    private readonly DataProvider _dataProvider;
    private readonly JsonPartialDataInitialiser _partialDataInitialiser;
    private bool _isInitialised;

    public JsonDataInitialiser(IDataLoader dataLoader,DataProvider dataProvider, JsonPartialDataInitialiser partialDataInitialiser)
    {
        _dataLoader = dataLoader;
        _dataProvider = dataProvider;
        _partialDataInitialiser = partialDataInitialiser;
    }

    public async Task InitialiseDataAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialised)
        {
            return;
        }

        _isInitialised = true;

        await _partialDataInitialiser.InitialiseVersionDataAsync(cancellationToken);
        await _partialDataInitialiser.InitialiseRestAsync(cancellationToken);
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