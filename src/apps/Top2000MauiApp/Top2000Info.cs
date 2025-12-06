namespace Top2000MauiApp;

public static partial class MauiProgram
{
    public class Top2000Info
    {
        public static bool IsLive()
        {
            var current = DateTime.UtcNow;

            var first = new DateTime(current.Year, 12, 24, 23, 0, 0, DateTimeKind.Utc); // first day of Christmas for CET in UTC time
            var last = new DateTime(current.Year, 12, 31, 23, 0, 0, DateTimeKind.Utc); // new year for CET in UTC time

            return current > first && current < last;
        }

    }
}
