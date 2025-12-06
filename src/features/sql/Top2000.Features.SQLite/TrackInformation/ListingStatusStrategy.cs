using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite.TrackInformation;

public class ListingStatusStrategy
{
    private readonly int recordedYear;
    private readonly List<ListingInformation> previous = [];

    public ListingStatusStrategy(int recordedYear)
    {
        this.recordedYear = recordedYear;
    }

    public ListingStatus Determine(ListingInformation current)
    {
        var status = GetStatus(current);
        previous.Add(current);
        return status;
    }

    private ListingStatus GetStatus(ListingInformation current)
    {
        if (!current.CouldBeListed(recordedYear))
        {
            return ListingStatus.NotAvailable;
        }

        if (!current.Position.HasValue)
        {
            return ListingStatus.NotListed;
        }

        if (!previous.Exists(x => x.Status == ListingStatus.New))
        {
            return ListingStatus.New;
        }

        if (!current.Offset.HasValue)
        {
            return ListingStatus.Back;
        }

        if (current.Offset == 0)
        {
            return ListingStatus.Unchanged;
        }

        if (current.Offset < 0)
        {
            return ListingStatus.Increased;
        }

        if (current.Offset > 0)
        {
            return ListingStatus.Decreased;
        }

        // let the fallback be New, instead of a InvalidOperationException. 
        return ListingStatus.New;
    }
}
