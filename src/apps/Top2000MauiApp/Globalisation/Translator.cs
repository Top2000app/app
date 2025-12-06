using System.ComponentModel;

namespace Top2000MauiApp.Globalisation;

public class Translator : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public static Translator Instance { get; } = new Translator();

    public string this[string text]
    {
        get
        {
            if (text == "text") { return $"%{text}%"; }
            string? value = null;
            try
            {
                var manager = AppResources.ResourceManager;

                value = manager.GetString(text, AppResources.Culture);
            }
            catch (FileNotFoundException)
            {
                // not sure why this happens... seems not to matter
            }

            return string.IsNullOrWhiteSpace(value)
                ? "%" + text + "%"
                : value;
        }
    }

    public void Invalidate()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
