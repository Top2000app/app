namespace Top2000.Features.AllListingsOfEdition;

public static class GroupExtensions
{
    /// <summary>
    /// Grouped the Tracks by the PlayUtcDateAndTime without the minute and second compontent
    /// </summary>
    /// <param name="tracks">IEnumerable of tracs to group</param>
    /// <returns>Grouped IEnumerable of TrackListing by Year/Month/Day/Hour in Utc Time</returns>
    public static IEnumerable<IGrouping<DateTime, TrackListing>> GroupByPlayUtcDateAndTime(this IEnumerable<TrackListing> tracks)
    {
        return tracks.GroupBy(x => new DateTime(
            x.PlayUtcDateAndTime.Year,
            x.PlayUtcDateAndTime.Month,
            x.PlayUtcDateAndTime.Day,
            x.PlayUtcDateAndTime.Hour, 0, 0, DateTimeKind.Utc));
    }

    /// <summary>
    /// Group the the track by their positions in groups of 100.
    /// 100 is put in the 100 - 200 group
    /// 2000 is put in the 1900 - 2000 group but only if the count of items is 2000 otherwise it is grouped in the 2000 - 2100 group
    /// </summary>
    /// <param name="tracks">IEnumerable of tracs to group</param>
    /// <returns>Grouped IEnumerable of TrackListing by Position</returns>
    public static IEnumerable<IGrouping<string, TrackListing>> GroupByPosition(this IEnumerable<TrackListing> tracks)
    {
        var count = tracks.Count();

        return tracks.GroupBy(x => Position(x, count));
    }

    private static string Position(TrackListing listing, int countOfItems)
    {
        const int GroupSize = 100;

        if (listing.Position < 100)
        {
            return "1 - 100";
        }

        if (countOfItems > 2000)
        {
            if (listing.Position >= 2400)
            {
                return "2400 - 2500";
            }
        }
        else
        {
            if (listing.Position >= 1900)
            {
                return "1900 - 2000";
            }
        }

        var min = listing.Position / GroupSize * GroupSize;
        var max = min + GroupSize;

        return $"{min} - {max}";
    }
}
