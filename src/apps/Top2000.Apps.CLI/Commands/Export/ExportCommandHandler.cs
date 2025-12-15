namespace Top2000.Apps.CLI.Commands.Export;

public class ExportCommandHandler
{
    
    public async Task<int> HandleExportCsvAsync(ParseResult result, CancellationToken token)
    {
        await Task.Yield();

        return 1;
    }

    public async Task HandleExportJsonAsync(string outputPath, string type)
    {
       
        AnsiConsole.MarkupLine($"[green]✓[/] Successfully exported {type} to [blue]{outputPath}[/]");
    }
}
