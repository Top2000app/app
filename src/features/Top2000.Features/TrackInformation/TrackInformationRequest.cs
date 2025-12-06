namespace Top2000.Features.TrackInformation;

public class TrackInformationRequest : IRequest<TrackDetails>
{
    public required int TrackId { get; init; }
}
