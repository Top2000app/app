using Top2000MauiApp.Common;

namespace Top2000MauiApp.Pages.Overview.Date;

public class DateTimeKeyToString : ValueConverterBase<DateTime, string>
{
    public override string Convert(DateTime value)
    {
        var hour = value.Hour + 1;
        var date = value.ToString("dddd dd MMM H", App.DateTimeFormatProvider);

        return $"{date}:00 - {hour}:00";
    }
}
