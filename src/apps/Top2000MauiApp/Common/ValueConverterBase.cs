#nullable disable

using System.Globalization;

namespace Top2000MauiApp.Common;

public abstract class ValueConverterBase<TIn, TOut> : ValueConverterBase<TIn, TOut, object>
{
    public abstract TOut Convert(TIn value);

    public override TOut Convert(TIn value, object param) => this.Convert(value);
}

public abstract class ValueConverterBase<TIn, TOut, TParam> : IValueConverter
{
    protected CultureInfo Culture { get; private set; }

    protected Type TargetType { get; private set; }

    public abstract TOut Convert(TIn value, TParam param);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is TIn))
        {
            return default(TIn);
        }

        this.Culture = culture;
        this.TargetType = targetType;

        return this.Convert((TIn)value, (TParam)parameter);
    }

    public virtual TIn ConvertBack(TOut value, TParam param)
    {
        throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        this.Culture = culture;
        this.TargetType = targetType;

        return this.ConvertBack((TOut)value, (TParam)parameter);
    }
}
