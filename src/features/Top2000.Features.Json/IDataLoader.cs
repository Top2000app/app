namespace Top2000.Features.Json;

public interface IDataLoader
{
    Task<Stream> LoadDataAsync(CancellationToken cancellationToken = default);
}