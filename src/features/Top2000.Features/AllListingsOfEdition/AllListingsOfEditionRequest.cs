namespace Top2000.Features.AllListingsOfEdition;

public sealed class AllListingsOfEditionRequest : IRequest<HashSet<TrackListing>>
{
    public required int Year { get; init; }
}
