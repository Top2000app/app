namespace Top2000.Data.StaticApiGenerator;

[DebuggerDisplay("{FileName}")]
public sealed class VersionFile
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