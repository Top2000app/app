using System.Reflection;
using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Info;

public class InfoCommand(Top2000Services top2000Services) : CommandBase("--info", "Displays information about the application and database")
{
    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "Unknown";
        var databaseVersion = await top2000Services.DataVersion(token);
        
        AnsiConsole.MarkupLine($"Top 2000 CLI Application");
        AnsiConsole.MarkupLine($"Version: [yellow]{version}[/]");
        AnsiConsole.MarkupLine($"Database: [yellow]{databaseVersion}[/]");
    }
}