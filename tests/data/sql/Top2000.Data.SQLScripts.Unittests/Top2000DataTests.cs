using AwesomeAssertions;

[assembly: Parallelize]

namespace Top2000.Data.Tests;

[TestClass]
public class Top2000DataTests
{
    private readonly Top2000Data _sut = new();

    [TestMethod]
    public void DataAssemblyIsTheAssemblyOfTheTop2000Data()
    {
        _sut.DataAssembly.Should().BeSameAs(typeof(Top2000Data).Assembly);
    }

    [TestMethod]
    public async Task AllSqlFilesCanBeReadAsync()
    {
        var filesAsTask = _sut.GetAllSqlFiles()
            .Select(GetNameContentAsync);

        var files = await Task.WhenAll(filesAsTask);

        foreach (var (name, content) in files)
        {
            content.Should().NotBeNullOrWhiteSpace($"the file '{name}' does not have content");
        }
    }

    [TestMethod]
    public void AllSqlFileCanBeStreamed()
    {
        var filesAsStream = _sut.GetAllSqlFiles()
            .Select(_sut.GetScriptStream)
            .ToList();

        foreach (var item in filesAsStream)
        {
            item.Should().NotBeNull();
            item.Dispose();
        }
    }

    [TestMethod]
    public void FileNamesDoNotContainSpaces()
    {
        var fileNames = _sut.GetAllSqlFiles()
            .ToList();

        foreach (var fileName in fileNames)
        {
            fileName.Should().NotContain(" ", $"the file {fileName} cannot contain a space");
        }
    }

    private async Task<(string name, string content)> GetNameContentAsync(string fileName)
    {
        var content = await _sut.GetScriptContentAsync(fileName);
        return (fileName, content);
    }
}