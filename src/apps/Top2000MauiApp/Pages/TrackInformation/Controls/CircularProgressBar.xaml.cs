namespace Top2000MauiApp.Pages.TrackInformation.Controls;

public partial class CircularProgressBar : ContentView
{
    public CircularProgressBar()
    {
        InitializeComponent();
        this.View.Invalidate();
    }

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(int), typeof(CircularProgressBar), propertyChanged: ProgressChanged);

    private static void ProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularProgressBar control)
        {
            control.ProgressBar.Progress = (int)newValue;
            control.View.Invalidate();
        }
    }

    public int Progress
    {
        get { return (int)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }
}