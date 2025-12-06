namespace Top2000.Data.ClientDatabase;

public class Top2000AssemblyDataSource : ISource
{
    private readonly ITop2000AssemblyData _top2000Data;

    public Top2000AssemblyDataSource(ITop2000AssemblyData top2000Data)
    {
        _top2000Data = top2000Data;
    }

    public Task<ImmutableSortedSet<string>> ExecutableScriptsAsync(ImmutableSortedSet<string> journals)
    {
        var scripts = _top2000Data.GetAllSqlFiles()
            .Except(journals)
            .ToImmutableSortedSet();

        return Task.FromResult(scripts);
    }

    public async Task<SqlScript> ScriptContentsAsync(string scriptName)
    {
        var contents = await _top2000Data.GetScriptContentAsync(scriptName);
        return new SqlScript(scriptName, contents);
    }
}
