using Microsoft.Data.Sqlite;
using Top2000.Features.AllEditions;

namespace Top2000.Features.SQLite.AllEditions;

public class AllEditionsRequestHandler : IRequestHandler<AllEditionsRequest, SortedSet<Edition>>
{
    private readonly SqliteConnection connection;

    public AllEditionsRequestHandler(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public async Task<SortedSet<Edition>> Handle(AllEditionsRequest request, CancellationToken cancellationToken)
    {
        var editions = new List<Edition>();
        await connection.OpenAsync(cancellationToken);
        await using var cmd = connection.CreateCommand();
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
