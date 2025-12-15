namespace Top2000.Apps.CLI.Commands.Export.csv;

public static class ExportCsvCommand
{
    public static Command CreateCommand(ExportCsvCommandHandler csvCommandHandler)
    {
        var csvCommand = new Command("csv", "Export to CSV format");
        var outputOption = new Option<string>(name: "--output", "-o", "/o")
        {
            Description = "Output file path",
        };
        csvCommand.Add(outputOption);
        
        csvCommand.SetAction(csvCommandHandler.HandleExportCsvAsync);

        return csvCommand;
    }
}