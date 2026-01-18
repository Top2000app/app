using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.Views;

public class TrackDetailsControl : TemplatedControl
{
    public static readonly StyledProperty<TrackDetails> ValueProperty =
        AvaloniaProperty.Register<TrackDetailsControl, TrackDetails>(
            nameof(Value),
            defaultBindingMode: BindingMode.TwoWay);

    public TrackDetails Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}