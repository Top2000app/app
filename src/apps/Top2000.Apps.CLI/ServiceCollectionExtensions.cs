using Microsoft.Extensions.DependencyInjection;
using Top2000.Apps.CLI.Commands;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRootCommand<TCommand>()
            where TCommand : RootCommand
        {
            services.AddSingleton<TCommand>();
            return services;
        }
        
        public IServiceCollection AddCommand<TCommand>() where TCommand : CommandBase
        {
            services.AddKeyedSingleton<CommandBase, TCommand>(typeof(Top2000Command));
            return services;
        }
        
        public IServiceCollection AddCommand<TParent, TCommand>() where TCommand : CommandBase
        {
            services.AddKeyedSingleton<CommandBase, TCommand>(typeof(TParent));
            return services;
        }
    }
}   