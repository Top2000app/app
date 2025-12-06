using System.Collections.Immutable;
using Top2000.Data.ClientDatabase;

namespace Top2000.Features.Specs.Bindings;

public class Top2000AssemblyDataSourceSpecDecorator : ISource
{
    private readonly Top2000AssemblyDataSource _component;
    private readonly int _skip;

    public Top2000AssemblyDataSourceSpecDecorator(Top2000AssemblyDataSource component, int skip)
    {
        this._component = component;
        this._skip = skip;
    }

    public async Task<ImmutableSortedSet<string>> ExecutableScriptsAsync(ImmutableSortedSet<string> journals)
    {
        return (await _component.ExecutableScriptsAsync(journals))
            .SkipLast(_skip)
            .ToImmutableSortedSet();
    }

    public Task<SqlScript> ScriptContentsAsync(string scriptName)
    {
        return _component.ScriptContentsAsync(scriptName);
    }
}
