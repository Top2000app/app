namespace DownloaderApp;

public sealed class ListingStatusStrategy
{
    private readonly int _recordedYear;
    private readonly List<ListingInformation> _previous = new();

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
            return ListingStatus.NotAvailable;

        if (!current.Position.HasValue)
            return ListingStatus.NotListed;

        if (_previous.All(x => x.Status != ListingStatus.New))
            return ListingStatus.New;

        if (!current.Offset.HasValue)
            return ListingStatus.Back;

        if (current.Offset == 0)
            return ListingStatus.Unchanged;

        if (current.Offset < 0)
            return ListingStatus.Increased;

        if (current.Offset > 0)
            return ListingStatus.Decreased;

        throw new InvalidOperationException();
    }
}