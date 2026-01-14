using Microsoft.Extensions.DependencyInjection;

namespace Top2000.Apps.CLI.Commands;

public abstract class CommandBase(string name, string? description = null)
{
    protected virtual List<Symbol> Symbols => [];

    public Command Create(IServiceProvider serviceProvider)
    {
        var command = new Command(name, description);

        foreach (var symbol in Symbols)
        {
            switch (symbol)
            {
                case Option options:
                    command.Add(options);
                    break;
                case Argument argument:
                    command.Add(argument);
                    break;
            }
        }

        var children = serviceProvider.GetKeyedServices<CommandBase>(this.GetType());
        foreach (var child in children)
        {
            command.Add(child.Create(serviceProvider));
        }
        
        command.SetAction(ExecuteAsync);
        
        return command;
    }

    protected virtual Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}