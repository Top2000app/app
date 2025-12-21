namespace Top2000.Features.Listings;

public static class GroupExtensions
{
    /// <param name="tracks">IEnumerable of tracks to group</param>
    extension(IEnumerable<TrackListing> tracks)
    {
        /// <summary>
        /// Grouped the Tracks by the PlayUtcDateAndTime without the minute and second component
        /// </summary>
        /// <returns>Grouped IEnumerable of TrackListing by Year/Month/Day/Hour in Utc Time</returns>
        public IEnumerable<IGrouping<DateTime, TrackListing>> GroupByPlayUtcDateAndTime()
        {
            return tracks.GroupBy(x => new DateTime(
                x.PlayUtcDateAndTime.Year,
                x.PlayUtcDateAndTime.Month,
                x.PlayUtcDateAndTime.Day,
                x.PlayUtcDateAndTime.Hour, 0, 0, DateTimeKind.Utc));
        }

        /// <summary>
        /// Group the track by their positions in groups of 100.
        /// 100 is put in the 100 - 200 group
        /// 2000 is put in the 1900 - 2000 group but only if the count of items is 2000 otherwise it is grouped in the 2000 - 2100 group
        /// </summary>
        /// <returns>Grouped IEnumerable of TrackListing by Position</returns>
        public IEnumerable<IGrouping<string, TrackListing>> GroupByPosition()
        {
            var count = tracks.Count();

            return tracks.GroupBy(x => Position(x, count));
        }
    }

    private static string Position(TrackListing listing, int countOfItems)
    {
        const int groupSize = 100;

        if (listing.Position < groupSize)
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

        var min = listing.Position / groupSize * groupSize;
        var max = min + groupSize;

        return $"{min} - {max}";
    }
}
