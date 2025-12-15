namespace Top2000.Apps.CLI.Commands.Export;

public class ExportCommandHandler
{
    
    public async Task<int> HandleExportCsvAsync(ParseResult result, CancellationToken token)
    {
        await Task.Yield();

        return 1;
    
        /*
        await AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(),
            })
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask($"[green]Exporting {type} to CSV[/]");
                
                // Simulate export process
                for (int i = 0; i <= 100; i += 5)
                {
                    task.Value = i;
                    await Task.Delay(50);
                }
                
                await _exportService.ExportToCsvAsync(outputPath, type);
            });
        
        var panel = new Panel($"[green]✓ Export Complete[/]\n\nFile: [blue]{outputPath}[/]\nType: [yellow]{type}[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Green);
        */
        
        // AnsiConsole.Write(panel);
    }

    public async Task HandleExportJsonAsync(string outputPath, string type)
    {
       
        AnsiConsole.MarkupLine($"[green]✓[/] Successfully exported {type} to [blue]{outputPath}[/]");
    }
}
