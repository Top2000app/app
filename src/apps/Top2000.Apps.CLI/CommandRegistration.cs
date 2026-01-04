using Microsoft.Extensions.DependencyInjection;
using Top2000.Apps.CLI.Commands;

internal class CommandRegistration<TCommand> where TCommand : class, ICommand
{
    private readonly IServiceCollection _services;

    public CommandRegistration(IServiceCollection services)
    {
        _services = services;
    }
    
    public CommandRegistration<TCommand> AddSubCommand<TSubCommand>() 
        where TSubCommand : class, ICommand<TCommand>
    {
        _services.AddSingleton<ICommand<TCommand>, TSubCommand>();
        return this;
    }
}