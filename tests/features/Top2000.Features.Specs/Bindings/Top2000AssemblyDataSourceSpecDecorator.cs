using System.Collections.Immutable;
using Top2000.Data.ClientDatabase;

namespace Top2000.Features.Specs.Bindings;

public class Top2000AssemblyDataSourceSpecDecorator : ISource
{
    private readonly Top2000AssemblyDataSource component;
    private readonly int skip;

    public Top2000AssemblyDataSourceSpecDecorator(Top2000AssemblyDataSource component, int skip)
    {
        this.component = component;
        this.skip = skip;
    }

    public async Task<ImmutableSortedSet<string>> ExecutableScriptsAsync(ImmutableSortedSet<string> journals)
    {
        return (await component.ExecutableScriptsAsync(journals))
            .SkipLast(skip)
            .ToImmutableSortedSet();
    }

    public Task<SqlScript> ScriptContentsAsync(string scriptName)
    {
        return component.ScriptContentsAsync(scriptName);
    }
}
