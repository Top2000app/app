using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Show;

public class ShowEditionsCommand : ICommand<ShowCommands>
{
    private readonly Top2000Services _services;

    public ShowEditionsCommand(Top2000Services services)
    {
        _services = services;
    }
    
    public Command Create()
    {
        var editionsCommand = new Command("editions", "Show Top 2000 editions");

        editionsCommand.SetAction(HandleShowEditionsAsync);

        return editionsCommand;
    }
    
    private async Task<int> HandleShowEditionsAsync(ParseResult result, CancellationToken token)
    {
        var editions = (await _services.AllEditionsAsync(token))
            .OrderBy(x => x.Year)
            .ToList();


        AnsiConsole.Clear();
        if (editions.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No editions found.[/]");
            return 0;
        }

        
        var table = new Table();
        table.AddColumn("[bold]#[/]");
        table.AddColumn("[bold]Year[/]");
        table.AddColumn("[bold]Start Date[/]");
        table.AddColumn("[bold]End Date[/]");
        table.AddColumn("[bold]Duration[/]");
        table.AddColumn("[bold]Has Play Time[/]");

        var index = 1;
        foreach (var edition in editions)
        {
            var startLocal = edition.StartUtcDateAndTime.ToLocalTime();
            var endLocal = edition.EndUtcDateAndTime.ToLocalTime();
            
            var duration = endLocal - startLocal;
            if (edition.Year == 2023)
            {
                var edition2024 = editions.FirstOrDefault(e => e.Year == 2024);
                if (edition2024 != null)
                {
                    var start2024Local = edition2024.StartUtcDateAndTime.ToLocalTime();
                    var end2024Local = edition2024.EndUtcDateAndTime.ToLocalTime();
                    var duration2024 = end2024Local - start2024Local;
                    duration = duration2024 + TimeSpan.FromHours(43.5);
                }
                else
                {
                    duration += TimeSpan.FromHours(43.5);
                }
            }
            
            var durationText = $"{duration.Days}d {duration.Hours}h {duration.Minutes}m";
            var hasPlayTime = edition.HasPlayDateAndTime ? "[green]✓[/]" : "[red]✗[/]";

            table.AddRow(
                index.ToString(),
                edition.Year.ToString(),
                startLocal.ToString("yyyy-MM-dd HH:mm"),
                endLocal.ToString("yyyy-MM-dd HH:mm"),
                durationText,
                hasPlayTime
            );
            
            index++;
        }

        AnsiConsole.Write(table);
        
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[dim]Found {editions.Count} edition(s)[/]");
        
        return 0;
    }
}