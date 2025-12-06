namespace Top2000.Features.TrackInformation;

public class TrackDetails
{
    public required string Title { get; init; }

    public required string Artist { get; init; }

    public required int RecordedYear { get; init; }

    public required SortedSet<ListingInformation> Listings { get; init;  }

    public ListingInformation Highest => Listings
        .Where(x => x.Position.HasValue)
        .OrderBy(x => x.Position)
        .ThenBy(x => x.Edition)
        .First();

    public ListingInformation Lowest => Listings
        .Where(x => x.Position.HasValue)
        .OrderBy(x => x.Position)
        .ThenBy(x => x.Edition)
        .Last();

    public ListingInformation First => Listings.Single(x => x.Status == ListingStatus.New);

    public ListingInformation Latest => Listings.First(x => x.Position.HasValue);

    public int Appearances => Listings.Count(x => x.Position.HasValue);

    public int AppearancesPossible => Listings.Count(x => x.Status != ListingStatus.NotAvailable);
}
