using Top2000.Features.AllEditions;

namespace Top2000.Features.SQLite.AllEditions;

public class EditionDescendingComparer : Comparer<Edition>
{
    public override int Compare(Edition? x, Edition? y)
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

        return y.Year.CompareTo(x.Year);
    }
}
