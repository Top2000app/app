namespace Top2000.Data.StaticApiGenerator;

public interface ITransformSqlFiles
{
    IReadOnlyCollection<VersionFile> Transform();
}

public sealed class SqlFileTransformer : ITransformSqlFiles
{
    private readonly ITop2000AssemblyData _top2000Data;

    public SqlFileTransformer(ITop2000AssemblyData top2000Data)
    {
        _top2000Data = top2000Data;
    }

    public IReadOnlyCollection<VersionFile> Transform()
    {
        var allVersions = _top2000Data
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
}