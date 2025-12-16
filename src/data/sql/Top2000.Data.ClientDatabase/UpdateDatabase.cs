namespace Top2000.Data.ClientDatabase;

public interface IUpdateClientDatabase
{
    Task RunAsync(ISource source);
}

public class UpdateDatabase : IUpdateClientDatabase
{
    private readonly SqliteConnection _connection;

    public UpdateDatabase(SqliteConnection connection)
    {
        this._connection = connection;
    }

    public async Task RunAsync(ISource source)
    {
        var journals = await AllJournalsAsync();
        var executableScripts = await source.ExecutableScriptsAsync(journals);

        foreach (var scriptName in executableScripts)
        {
            var script = await source.ScriptContentsAsync(scriptName);
            await ExecuteScriptAsync(script);
        }
    }

    private async Task ExecuteScriptAsync(SqlScript script)
    {
        await _connection.OpenAsync();
        await using var transaction = _connection.BeginTransaction();
        try
        {
            var sections = script.SqlSections();

            foreach (var section in sections)
            {
                 var cmd = _connection.CreateCommand();
                 cmd.CommandText = section;
                 cmd.Transaction = transaction;
                 await cmd.ExecuteNonQueryAsync();
            }

            await using var insertCmd = _connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO Journal (ScriptName) VALUES ($scriptName)";
            insertCmd.Parameters.AddWithValue("$scriptName", script.ScriptName);
            insertCmd.Transaction = transaction;

            await insertCmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    private async Task<ImmutableSortedSet<string>> AllJournalsAsync()
    {
        var scriptNames = ImmutableSortedSet.CreateBuilder<string>();
        await _connection.OpenAsync();

        await using (var ensureCmd = _connection.CreateCommand())
        {
            ensureCmd.CommandText = "CREATE TABLE IF NOT EXISTS Journal (ScriptName TEXT NOT NULL PRIMARY KEY)";
            await ensureCmd.ExecuteNonQueryAsync();
        }

        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT ScriptName FROM Journal";
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var scriptName = reader.GetString(0);
            scriptNames.Add(scriptName);
        }
        await _connection.CloseAsync();
        return scriptNames.ToImmutable();
    }
}
