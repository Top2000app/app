using Top2000.Features.Editions;

namespace Top2000.Features.SQLite.Editions;

public class EditionFeature : IEditions
{
    private readonly SqliteConnection _connection;

    public EditionFeature(SqliteConnection connection)
    {
        _connection = connection;
    }
    
    public async Task<SortedSet<Edition>> AllEditionsAsync(CancellationToken cancellationToken = default)
    {
        var editions = new List<Edition>();
        
        await _connection.OpenAsync(cancellationToken);
        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Year, StartUtcDateAndTime, EndUtcDateAndTime, HasPlayDateAndTime FROM Edition";
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            editions.Add(new Edition
            {
                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                StartUtcDateAndTime = reader.GetDateTime(reader.GetOrdinal("StartUtcDateAndTime")),
                EndUtcDateAndTime = reader.GetDateTime(reader.GetOrdinal("EndUtcDateAndTime")),
                HasPlayDateAndTime = reader.GetBoolean(reader.GetOrdinal("HasPlayDateAndTime")),
            });
        }

        return new SortedSet<Edition>(editions, new EditionDescendingComparer());
    }
}
