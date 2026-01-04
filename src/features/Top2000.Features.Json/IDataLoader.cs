namespace Top2000.Features.Json;

public interface IDataLoader
{
    Task<Stream> LoadDataVersionAsync();

    Task<Stream> LoadEditionDataAsync(int edition);
}