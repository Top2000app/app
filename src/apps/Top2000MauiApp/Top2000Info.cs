namespace Top2000MauiApp;

public static class Top2000Info
{
    public static bool ShowLiveScreen()
    {
        var current = DateTime.UtcNow;

        var first = new DateTime(current.Year, 12, 24, 12, 0, 0, DateTimeKind.Utc); // first day of Christmas for CET in UTC time
        var last = new DateTime(current.Year, 12, 31, 23, 0, 0, DateTimeKind.Utc); // new year for CET in UTC time

        return current > first && current < last;
    }
}
