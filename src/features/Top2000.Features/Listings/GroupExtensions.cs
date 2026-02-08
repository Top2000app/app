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
        /// Grouped the Tracks by the PlayUtcDateAndTime in local time without the minute and second component
        /// </summary>
        /// <returns>Grouped IEnumerable of TrackListing by Year/Month/Day/Hour in Local Time</returns>
        public IEnumerable<IGrouping<DateTime, TrackListing>> GroupByPlayLocalDateAndTime()
        {
            return tracks.GroupBy(x =>
            {
                var localTime = x.PlayUtcDateAndTime.ToLocalTime();
                return new DateTime(
                    localTime.Year,
                    localTime.Month,
                    localTime.Day,
                    localTime.Hour, 0, 0, DateTimeKind.Local);
            });
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

            return tracks.GroupBy(x => Position(x.Position, count));
        }
        
        /// <summary>
        /// Group the track by their positions in groups of 100.
        /// 100 is put in the 100 - 200 group
        /// 2000 is put in the 1900 - 2000 group but only if the count of items is 2000 otherwise it is grouped in the 2000 - 2100 group
        /// </summary>
        /// <returns>Grouped IEnumerable of TrackListing by Position</returns>
        public IEnumerable<IGrouping<string, TrackListing>> GroupByPosition(int count)
        {
            return tracks.GroupBy(x => Position(x.Position, count));
        }
        
        
    }

    public static string Position(int position, int countOfItems)
    {
        const int groupSize = 100;

        if (position < groupSize)
        {
            return "1 - 100";
        }

        if (countOfItems > 2000)
        {
            if (position >= 2400)
            {
                return "2400 - 2500";
            }
        }
        else
        {
            if (position >= 1900)
            {
                return "1900 - 2000";
            }
        }

        var min = position / groupSize * groupSize;
        var max = min + groupSize;

        return $"{min} - {max}";
    }
}
