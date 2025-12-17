namespace Top2000.Features.TrackInformation;

public interface ITrackInformation
{
    public Task<TrackDetails> TrackDetailsAsync(int trackId, CancellationToken cancellationToken = default);
}