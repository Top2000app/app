namespace Top2000.Apps.CLI.Commands.Export.Api;

public static class ExportApiCommand
{
    public static Command CreateCommand(ExportApiCommandHandler apiCommandHandler)
    {
        var csvCommand = new Command("api", "Export to static api format");
        var outputOption = new Option<string>(name: "--output", "-o", "/o")
        {
            Description = "Output file path",
        };
        csvCommand.Add(outputOption);
        
        csvCommand.SetAction(apiCommandHandler.HandleExportStaticSiteAsync);

        return csvCommand;
    }
}