namespace Top2000.Data.ClientDatabase;

public interface ISource
{
    Task<ImmutableSortedSet<string>> ExecutableScriptsAsync(ImmutableSortedSet<string> journals);

    Task<SqlScript> ScriptContentsAsync(string scriptName);
}
