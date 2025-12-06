using System.Reflection;
using System.Text;

namespace Top2000.Data;

public interface ITop2000AssemblyData
{
    Assembly DataAssembly { get; }

    Task<string> GetScriptContentAsync(string fileName);

    Stream GetScriptStream(string fileName);

    ISet<string> GetAllSqlFiles();
}

public class Top2000Data : ITop2000AssemblyData
{
    private readonly Func<string, bool> _isSqlFile = x => x.EndsWith(".sql", StringComparison.OrdinalIgnoreCase);
    private readonly string _prefix;

    public Top2000Data()
    {
        _prefix = DataAssembly.GetName().Name + ".sql.";
    }

    public Assembly DataAssembly => typeof(Top2000Data).Assembly;

    public async Task<string> GetScriptContentAsync(string fileName)
    {
        await using var stream = DataAssembly.GetManifestResourceStream(_prefix + fileName) 
                                 ?? throw new FileNotFoundException($"Unable to find {fileName} in {DataAssembly.GetName()}");
        
        using var reader = new StreamReader(stream, Encoding.UTF8);

        return await reader.ReadToEndAsync();
    }

    public Stream GetScriptStream(string fileName)
    {
        return DataAssembly.GetManifestResourceStream(_prefix + fileName)
            ?? throw new FileNotFoundException($"Unable to find {fileName} in {DataAssembly.GetName()}");
    }

    public ISet<string> GetAllSqlFiles()
    {
        return DataAssembly
            .GetManifestResourceNames()
            .Where(_isSqlFile)
            .Select(StripPrefixFromFileName)
            .ToHashSet();
    }

    private string StripPrefixFromFileName(string file) => file.Replace(_prefix, string.Empty);
}