using Top2000.Data.ClientDatabase;
using Top2000.Features.Data;

namespace Top2000.Features.SQLite.Data;

public class SqliteDataInitialiser : IDataInitialiser
{
    private readonly IUpdateClientDatabase _updateClientDatabase;
    private readonly Top2000AssemblyDataSource _assemblyDataSource;
    private readonly OnlineDataSource _onlineDataSource;

    public SqliteDataInitialiser(IUpdateClientDatabase updateClientDatabase, Top2000AssemblyDataSource assemblyDataSource, OnlineDataSource onlineDataSource)
    {
        _updateClientDatabase = updateClientDatabase;
        _assemblyDataSource = assemblyDataSource;
        _onlineDataSource = onlineDataSource;
    }
    
    public Task InitialiseDataAsync(CancellationToken cancellationToken = default)
    {
        return _updateClientDatabase.RunAsync(_assemblyDataSource);
    }
    
    public async Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        await _updateClientDatabase.RunAsync(_onlineDataSource);
    }
    
    public async Task<int> DataVersion(CancellationToken cancellationToken = default)
    {
        var versions = await _assemblyDataSource.ExecutableScriptsAsync([]);
        var lastVersion = int.Parse(versions.Last().Split('-')[0]);

        return lastVersion;
    }
}