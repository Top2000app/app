using System.Text.Json;
using System.Text.Json.Serialization;
using Top2000.Data.JsonClientDatabase;
using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Data;

namespace Top2000.Features.Json.Data;

public class JsonDataInitialiser : IDataInitialiser
{
    private readonly IDataLoader _dataLoader;
    private readonly Top2000DataContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonDataInitialiser(IDataLoader dataLoader, Top2000DataContext dbContext)
    {
        _dataLoader = dataLoader;
        _dbContext = dbContext;
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = new ShortNameNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
    
    public async Task InitialiseDataAsync(CancellationToken cancellationToken = default)
    {
        var dataStream = await _dataLoader.LoadDataAsync(cancellationToken);
        
        var data = await JsonSerializer.DeserializeAsync<Top2000DataContext>(dataStream, _jsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize JSON data into Top2000DataContext.");

        _dbContext.Editions = data.Editions;
        _dbContext.Listings = data.Listings;
        _dbContext.Tracks = data.Tracks;
    }

    public Task<int> DataVersion(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dbContext.Version);
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
        => InitialiseDataAsync(cancellationToken);
}