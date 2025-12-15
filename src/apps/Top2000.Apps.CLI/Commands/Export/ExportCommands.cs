using Top2000.Apps.CLI.Commands.Export.Api;
using Top2000.Apps.CLI.Commands.Export.Csv;
using Top2000.Apps.CLI.Commands.Export.Json;

namespace Top2000.Apps.CLI.Commands.Export;

public class ExportCommands : ICommand
{
    private readonly ExportJsonCommandHandler _jsonCommandHandler;
    private readonly ExportCsvCommandHandler _csvCommandHandler;
    private readonly ExportApiCommandHandler _apiCommandHandler;

    public ExportCommands(ExportJsonCommandHandler jsonCommandHandler, 
        ExportCsvCommandHandler csvCommandHandler,
        ExportApiCommandHandler apiCommandHandler)
    {
        _jsonCommandHandler = jsonCommandHandler;
        _csvCommandHandler = csvCommandHandler;
        _apiCommandHandler = apiCommandHandler;
    }

    public Command Create()
    {
        return new Command("export", "Export data to various formats")
        {
            ExportCsvCommand.CreateCommand(_csvCommandHandler),
            ExportJsonCommand.CreateCommand(_jsonCommandHandler),
            ExportApiCommand.CreateCommand(_apiCommandHandler)
        };
    }
}
