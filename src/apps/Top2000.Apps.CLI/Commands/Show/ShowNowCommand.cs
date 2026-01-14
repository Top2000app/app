using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Show;

public class ShowNowCommand(Top2000Services top2000Services) : CommandBase("now", "Show the currently playing Top 2000 song")
{
    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var currentList = await top2000Services.AllListingsOfEditionAsync(now.Year, token);
        var groupKey = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

        var tracks = currentList
            .Where(x => x.PlayUtcDateAndTime == groupKey)
            .ToList();

        if (tracks.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]The TOP2000 is not live at the moment.[/]");
        }
        else
        {
            TrackListView.DisplayTable(tracks, $"{groupKey.ToLocalTime():f} - {groupKey.ToLocalTime().AddHours(1):t}");
        }
    }
}