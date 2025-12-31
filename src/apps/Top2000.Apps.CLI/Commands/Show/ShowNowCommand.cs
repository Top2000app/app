using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Show;

public class ShowNowCommand : ICommand<ShowCommands>
{
    private readonly Top2000Services _top2000Services;

    public ShowNowCommand(Top2000Services top2000Services)
    {
        _top2000Services = top2000Services;
    }
    
    public Command Create()
    {
        var nowCommand = new Command("now", "Show the currently playing Top 2000 song");
        
        nowCommand.SetAction(HandleAsync);

        return nowCommand;
    }
    
    private async Task<int> HandleAsync(ParseResult result, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var currentList = await _top2000Services.AllListingsOfEditionAsync(now.Year, token);
        var groupKey = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

        var tracks = currentList
            .Where(x => x.PlayUtcDateAndTime == groupKey)
            .ToList();

        TrackListView.DisplayTable(tracks, $"{groupKey.ToLocalTime():f} - {groupKey.ToLocalTime().AddHours(1):t}");

        return 1;
    }


}