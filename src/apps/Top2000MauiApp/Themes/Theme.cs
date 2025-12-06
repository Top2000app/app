namespace Top2000MauiApp.Themes
{
    public interface IThemeService
    {
        string CurrentThemeName { get; }

        void SetTheme(string name);

        void SetThemeFromSetting();
    }

    public class ThemeService : IThemeService
    {
        private const string ThemePreferenceName = "Theme";

        public ThemeService()
        {
            CurrentThemeName = Light.ThemeName;
        }

        public string CurrentThemeName { get; private set; }

        public void SetThemeFromSetting()
        {
            if (Preferences.ContainsKey(ThemePreferenceName))
            {
                var name = Preferences.Get(ThemePreferenceName, "Light");
                SetTheme(name);
            }
        }

        public void SetTheme(string name)
        {
            var mergedDictionaries = Application.Current?.Resources?.MergedDictionaries;
            if (mergedDictionaries is not null)
            {
                mergedDictionaries.Clear();
                mergedDictionaries.Add(GetThemeByName(name));

                CurrentThemeName = name;
                Preferences.Set(ThemePreferenceName, name);
            }
        }

        private ResourceDictionary GetThemeByName(string name)
        {
            return name switch
            {
                Dark.ThemeName => new Dark(),
                _ => new Light(),
            };
        }
    }
}
