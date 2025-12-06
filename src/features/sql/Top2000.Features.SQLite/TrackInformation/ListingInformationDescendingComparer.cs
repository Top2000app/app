using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite.TrackInformation;

public class ListingInformationDescendingComparer : Comparer<ListingInformation>
{
    public override int Compare(ListingInformation? x, ListingInformation? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }
        if (x == null)
        {
            return 1;
        }
        if (y == null)
        {
            return -1;
        }

        return y.Edition.CompareTo(x.Edition);
    }
}
