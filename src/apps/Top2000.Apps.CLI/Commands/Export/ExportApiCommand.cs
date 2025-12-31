using System.Diagnostics;
using System.Text.Json;
using Top2000.Data;

namespace Top2000.Apps.CLI.Commands.Export;

public class ExportApiCommand : ICommand<ExportCommands>
{
    private readonly ITop2000AssemblyData _top2000AssemblyData;
    private static readonly JsonSerializerOptions SerializerSettings = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public ExportApiCommand(ITop2000AssemblyData top2000AssemblyData)
    {
        _top2000AssemblyData = top2000AssemblyData;
    }
    
    public Command Create()
    {
        var apiCommand = new Command("api", "Export to static api format");
        var outputOption = new Option<string>(name: "--output", "-o", "/o")
        {
            Description = "Output file path",
        };
        apiCommand.Add(outputOption);
        
        apiCommand.SetAction(HandleExportStaticSiteAsync);

        return apiCommand;
    }
    
    private async Task<int> HandleExportStaticSiteAsync(ParseResult result, CancellationToken token)
    {
        var outputPath = result.GetValue<string>("--output") ?? "site";

        AnsiConsole.MarkupLine($"[green]Exporting static site to:[/] [cyan]{outputPath}[/]");
        AnsiConsole.WriteLine();

        await CreateDataFilesAsync(outputPath);
        await CreateApiFileAsync(outputPath);
        
        AnsiConsole.MarkupLine("[green]✓ Static site export completed successfully![/]");
        
        return 0;
    }
    
    private async Task CreateDataFilesAsync(string location)
    {
        var toUpload = _top2000AssemblyData
            .GetAllSqlFiles()
            .ToList();

        AnsiConsole.MarkupLine($"[yellow]Creating SQL data files...[/] ([cyan]{toUpload.Count}[/] files)");
        
        var sqlPath = Path.Combine(location, "sql");
        if (!Directory.Exists(sqlPath))
        {
            Directory.CreateDirectory(sqlPath);
        }

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[green]Exporting SQL files[/]", maxValue: toUpload.Count);

                foreach (var file in toUpload)
                {
                    var contents = await _top2000AssemblyData.GetScriptContentAsync(file);
                    var fileName = Path.Combine(sqlPath, file);

                    await File.WriteAllTextAsync(fileName, contents);
                    
                    task.Description = $"[green]Exporting SQL files[/] - {file}";
                    task.Increment(1);
                }
            });

        AnsiConsole.MarkupLine($"[green]✓ SQL data files created[/] ([cyan]{toUpload.Count}[/] files written to [cyan]{sqlPath}[/])");
    }

    private async Task CreateApiFileAsync(string location)
    {
        var versions = Transform();

        AnsiConsole.MarkupLine($"[yellow]Creating API version files...[/] ([cyan]{versions.Count}[/] versions)");

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[blue]Generating version API files[/]", maxValue: versions.Count);

                foreach (var version in versions)
                {
                    var versionPath = Path.Combine(location, "versions", version.Version);
                    var json = JsonSerializer.Serialize(
                        version.Upgrades.Select(x => x.FileName), 
                        SerializerSettings);

                    Directory.CreateDirectory(versionPath);

                    var fileName = Path.Combine(versionPath, "upgrades");
                    await File.WriteAllTextAsync(fileName, json).ConfigureAwait(false);
                    
                    task.Description = $"[blue]Generating version API files[/] - {version.Version}";
                    task.Increment(1);
                }
            });

        AnsiConsole.MarkupLine($"[green]✓ API version files created[/] ([cyan]{versions.Count}[/] version endpoints generated)");
    }
    
    private List<VersionFile> Transform()
    {
        var allVersions = _top2000AssemblyData
            .GetAllSqlFiles()
            .Select(x => new VersionFile(x))
            .ToList();

        var allVersionsCopy = allVersions.ToList();

        foreach (var version in allVersions)
        {
            allVersionsCopy.Remove(version);
            version.AddRange(allVersionsCopy);
        }

        return allVersions;
    }

    [DebuggerDisplay("{FileName}")]
    private sealed class VersionFile
    {
        private readonly List<VersionFile> _upgrades;

        public VersionFile(string fileName)
        {
            _upgrades = [];
            Version = fileName.Split('-')[0];
            FileName = fileName;
        }

        public string Version { get; set; }

        public string FileName { get; set; }

        public IReadOnlyCollection<VersionFile> Upgrades => _upgrades;

        public void AddRange(IEnumerable<VersionFile> versionFiles)
        {
            _upgrades.AddRange(versionFiles);
        }
    }

}