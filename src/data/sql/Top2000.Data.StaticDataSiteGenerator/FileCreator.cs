namespace Top2000.Data.StaticApiGenerator;

public interface IFileCreator
{
    Task CreateDataFilesAsync(string location);

    Task CreateApiFileAsync(string location);
}

public sealed class FileCreator : IFileCreator
{
    private static readonly JsonSerializerOptions SerializerSettings = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ILogger<FileCreator> _logger;
    private readonly ITransformSqlFiles _transformer;
    private readonly ITop2000AssemblyData _top2000Data;

    public FileCreator(ILogger<FileCreator> logger, ITransformSqlFiles transformer, ITop2000AssemblyData top2000Data)
    {
        _logger = logger;
        _transformer = transformer;
        _top2000Data = top2000Data;
    }

    public async Task CreateDataFilesAsync(string location)
    {
        var toUpload = _top2000Data
            .GetAllSqlFiles()
            .ToList();

        foreach (var file in toUpload)
        {
            FileCreatorLog.SavingFileToDisk(_logger, file);

            var contents = await _top2000Data.GetScriptContentAsync(file);
            var path = Path.Combine(location, "sql");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.Combine(path, file);

            await File.WriteAllTextAsync(fileName, contents);
        }
    }

    public async Task CreateApiFileAsync(string location)
    {
        var versions = _transformer.Transform();

        foreach (var version in versions)
        {
            FileCreatorLog.SavingVersionToDisk(_logger, version.Version);

            var path = Path.Combine(location, "versions", version.Version);
            var json = JsonSerializer.Serialize(
                version.Upgrades.Select(x => x.FileName), 
                SerializerSettings);

            Directory.CreateDirectory(path);

            var fileName = Path.Combine(path, "upgrades");

            await File.WriteAllTextAsync(fileName, json).ConfigureAwait(false);
        }
    }
}

public static partial class FileCreatorLog
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Saving {File} to disk")]
    public static partial void SavingFileToDisk(this ILogger<FileCreator> logger, string file);

    [LoggerMessage(Level = LogLevel.Information, Message = "Saving version {Version} to disk")]
    public static partial void SavingVersionToDisk(this ILogger<FileCreator> logger, string version);
}
