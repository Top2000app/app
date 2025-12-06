using System.Globalization;
using Top2000MauiApp.Common;

namespace Top2000MauiApp.Pages.Overview.Date;

public class DateTimeToDateOnlyString : ValueConverterBase<DateTime, string>
{
    private static readonly IFormatProvider formatProvider = DateTimeFormatInfo.InvariantInfo;

    public override string Convert(DateTime value)
    {
        return value.ToString("dddd dd MMMM", formatProvider);
    }
}
