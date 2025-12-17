namespace Top2000.Features.Editions;

public interface IEditions
{
    public Task<SortedSet<Edition>> AllEditionsAsync(CancellationToken cancellationToken = default);
}
