namespace Top2000.Features.TrackInformation;

public class ListingStatusStrategy
{
    private readonly int _recordedYear;
    private readonly List<ListingInformation> _previous = [];

    public ListingStatusStrategy(int recordedYear)
    {
        _recordedYear = recordedYear;
    }

    public ListingStatus Determine(ListingInformation current)
    {
        var status = GetStatus(current);
        _previous.Add(current);
        return status;
    }

    private ListingStatus GetStatus(ListingInformation current)
    {
        if (!current.CouldBeListed(_recordedYear))
        {
            return ListingStatus.NotAvailable;
        }

        if (!current.Position.HasValue)
        {
            return ListingStatus.NotListed;
        }

        if (!_previous.Exists(x => x.Status == ListingStatus.New))
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
