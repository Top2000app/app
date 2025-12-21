namespace Top2000.Features.Data;

public interface IDataInitialiser
{
    Task InitialiseDataAsync(CancellationToken cancellationToken = default);
    Task<int> DataVersion(CancellationToken cancellationToken = default);
    Task UpdateAsync(CancellationToken cancellationToken = default);
}