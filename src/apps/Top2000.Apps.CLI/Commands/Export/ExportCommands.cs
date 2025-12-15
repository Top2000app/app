namespace Top2000.Apps.CLI.Commands.Export;

public class ExportCommands : ICommand
{
    private readonly ExportCommandHandler _handler;

    public ExportCommands(ExportCommandHandler handler)
    {
        _handler = handler;
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
        
        csvCommand.SetAction(_handler.HandleExportCsvAsync);

        return csvCommand;
    }
/*
    private static Command CreateJsonCommand(ExportCommandHandler handler)
    {
        var jsonCommand = new Command("json", "Export to JSON format");
        
        var outputOption = new Option<string>(
            name: "--output",
            description: "Output file path");
        outputOption.AddAlias("-o");
        
        var typeOption = new Option<string>(
            name: "--type",
            description: "Type of data to export (songs, artists, all)",
            getDefaultValue: () => "songs");
        typeOption.AddAlias("-t");

        jsonCommand.AddOption(outputOption);
        jsonCommand.AddOption(typeOption);
        
        jsonCommand.SetHandler(async (string? output, string type) =>
        {
            await handler.HandleExportJsonAsync(output ?? "top2000_export.json", type);
        }, outputOption, typeOption);
        
        return jsonCommand;
    }
    */
}
