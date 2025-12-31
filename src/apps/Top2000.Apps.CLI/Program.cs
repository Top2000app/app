using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Top2000.Apps.CLI.Commands.Show;
using Top2000.Apps.CLI.Commands.Export;
using Top2000.Apps.CLI.Commands;
using Top2000.Apps.CLI.Commands.Search;
using Top2000.Apps.CLI.Database;
using Top2000.Features;
using Top2000.Features.SQLite;

var host = Host.CreateApplicationBuilder(args);

host.Services
    .AddTop2000Features<SqliteFeatureAdapter>()
    .AddDbContext<Top2000DbContext>()
    .AddSingleton<Top2000Command>();

host.Services
    .AddCommand<ExportCommands>()
    .AddSubCommand<ExportJsonCommand>()
    .AddSubCommand<ExportApiCommand>()
    .AddSubCommand<ExportCsvCommand>();

host.Services
    .AddSingleton<ShowListingCommand>()
    .AddCommand<ShowCommands>()
    .AddSubCommand<ShowNowCommand>()
    .AddSubCommand<ShowEditionsCommand>()
    .AddSubCommand<ShowListingCommand>()
    ;
    
host.Services
    .AddSingleton<SearchCommandHandler>()
    .AddSingleton<ICommand, SearchCommand>() 
    ;

var app = host.Build();

await app.Services
    .GetRequiredService<Top2000Command>()
    .RunAsync(args);

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