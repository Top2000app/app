using Top2000.Apps.CLI.Commands.Export.csv;
using Top2000.Apps.CLI.Commands.Export.json;

namespace Top2000.Apps.CLI.Commands.Export;

public class ExportCommands : ICommand
{
    private readonly ExportJsonCommandHandler _jsonCommandHandler;
    private readonly ExportCsvCommandHandler _csvCommandHandler;

    public ExportCommands(ExportJsonCommandHandler jsonCommandHandler, ExportCsvCommandHandler csvCommandHandler)
    {
        _jsonCommandHandler = jsonCommandHandler;
        _csvCommandHandler = csvCommandHandler;
    }
    
    public Command Create()
    {
        return new Command("export", "Export data to various formats")
        {
            ExportCsvCommand.CreateCommand(_csvCommandHandler),
            ExportJsonCommand.CreateCommand(_jsonCommandHandler)
        };
    }
}
