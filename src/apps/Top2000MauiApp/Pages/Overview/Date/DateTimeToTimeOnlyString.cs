using Top2000MauiApp.Common;

namespace Top2000MauiApp.Pages.Overview.Date;

public class DateTimeToTimeOnlyString : ValueConverterBase<DateTime, string>
{
    public override string Convert(DateTime value)
    {
        var hour = value.Hour + 1;

        return $"{value.Hour}:00 - {hour}:00";
    }
}
