using Avalonia.Media;

namespace Top2000.Apps.AvaloniaApp.Assets;

public static class Colours
{
    public static readonly Color YellowColour = Color.FromRgb(255, 192, 0);
    public static readonly Color RedColour = Color.FromRgb(221, 48, 57);
    public static readonly Color GreenColour = Color.FromRgb(112, 173, 71);
    public static readonly Color GreyColour = Color.FromRgb(103, 103, 103);

    public static readonly Brush YellowColourBrush = new SolidColorBrush(YellowColour);
    public static readonly Brush RedColourBrush = new SolidColorBrush(RedColour);
    public static readonly Brush GreenColourBrush = new SolidColorBrush(GreenColour);
    public static readonly Brush GreyColourBrush = new SolidColorBrush(GreyColour);
}