using Microsoft.Extensions.DependencyInjection;
using Top2000.Apps.CLI.Commands;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public CommandRegistration<TCommand> AddCommand<TCommand>()
            where TCommand : class, ICommand
        {
            services.AddSingleton<ICommand, TCommand>();
            return new CommandRegistration<TCommand>(services);
        }
    }
}