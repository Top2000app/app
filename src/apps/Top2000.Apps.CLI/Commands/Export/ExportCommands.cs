using Top2000.Apps.CLI.Commands.Export.csv;

namespace Top2000.Apps.CLI.Commands.Export;

public class ExportCommands : ICommand
{
    private readonly ExportCommandHandler _handler;
    private readonly ExportCsvCommandHandler _csvCommandHandler;

    public ExportCommands(ExportCommandHandler handler, ExportCsvCommandHandler csvCommandHandler)
    {
        _handler = handler;
        _csvCommandHandler = csvCommandHandler;
    }
    
    public Command Create()
    {
        var exportCommand = new Command("export", "Export data to various formats");
        
        var csvCommand = CreateCsvCommand();
       // var jsonCommand = CreateJsonCommand(_handler);
        
        exportCommand.Add(csvCommand);
     //   exportCommand.Add(jsonCommand);
        
        return exportCommand;
    }
    
    private Command CreateCsvCommand()
    {
        var csvCommand = new Command("csv", "Export to CSV format");
        var outputOption = new Option<string>(name: "--output", "-o", "/o")
        {
            Description = "Output file path",
        };
        csvCommand.Add(outputOption);
        
        csvCommand.SetAction(_csvCommandHandler.HandleExportCsvAsync);

        return csvCommand;
    }
}
