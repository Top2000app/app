namespace Top2000.Data.StaticApiGenerator;

public sealed class PublishOnlyApplication : IRunApplication
{
    private readonly IFileCreator _fileCreator;

    public PublishOnlyApplication(IFileCreator fileCreator)
    {
        _fileCreator = fileCreator;
    }

    public async Task RunAsync()
    {
        const string location = $"_site";

        if (Directory.Exists(location))
        {
            Directory.Delete(location, recursive: true);
        }

        Directory.CreateDirectory(location);

        await Task.WhenAll
        (
            _fileCreator.CreateApiFileAsync(location),
            _fileCreator.CreateDataFilesAsync(location)
        );
    }
}