using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Top2000.Data.ClientDatabase;

public interface IUpdateClientDatabase
{
    Task RunAsync(ISource source);
}

public class UpdateDatabase : IUpdateClientDatabase
{
    private readonly Top2000DataOptions _configuration;

    public UpdateDatabase(IOptions<Top2000DataOptions> options)
    {
        _configuration = options.Value;
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
        var connection = new SqliteConnection(_configuration.ConnectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();
        try
        {
            var sections = script.SqlSections();

            foreach (var section in sections)
            {
                 var cmd = connection.CreateCommand();
                 cmd.CommandText = section;
                 cmd.Transaction = transaction;
                 await cmd.ExecuteNonQueryAsync();
            }

            await using var insertCmd = connection.CreateCommand();
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
            await connection.CloseAsync();
        }
    }

    private async Task<ImmutableSortedSet<string>> AllJournalsAsync()
    {
        var scriptNames = ImmutableSortedSet.CreateBuilder<string>();
        var connection = new SqliteConnection(_configuration.ConnectionString);
        await connection.OpenAsync();

        await using (var ensureCmd = connection.CreateCommand())
        {
            ensureCmd.CommandText = "CREATE TABLE IF NOT EXISTS Journal (ScriptName TEXT NOT NULL PRIMARY KEY)";
            await ensureCmd.ExecuteNonQueryAsync();
        }

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT ScriptName FROM Journal";
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var scriptName = reader.GetString(0);
            scriptNames.Add(scriptName);
        }
        await connection.CloseAsync();
        return scriptNames.ToImmutable();
    }
}
