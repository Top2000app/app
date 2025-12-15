namespace Top2000.Apps.CLI.Commands.Export.json;

public  static class ExportJsonCommand
{
    public static Command CreateCommand(ExportJsonCommandHandler csvCommandHandler)
    {
        var csvCommand = new Command("json", "Export to Json format");
        var outputOption = new Option<string>(name: "--output", "-o", "/o")
        {
            Description = "Output file path",
        };
        csvCommand.Add(outputOption);
        
        csvCommand.SetAction(csvCommandHandler.HandleExportJsonAsync);

        return csvCommand;
    }
}