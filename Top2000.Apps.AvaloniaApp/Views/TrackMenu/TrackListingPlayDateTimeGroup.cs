namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class TrackListingPlayDateTimeGroup : ITrackListingViewModel
{
    public string HeaderName => nameof(TrackListingPlayDateTimeGroup);
    public required string GroupName { get; init; }
    public required DateTime PlayTime { get; init; }
    
    public static string MakeNiceDateTimeString(DateTime dateTime)
    {
        var localTime = dateTime.ToLocalTime();
        return $"{localTime:dddd dd MMM HH:00}-{localTime.AddHours(1):H:00}";
    }
}