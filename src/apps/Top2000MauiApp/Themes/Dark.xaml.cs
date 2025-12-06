
using Microsoft.Maui.Controls.Xaml;

namespace Top2000MauiApp.Themes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dark : ResourceDictionary
    {
        public const string ThemeName = "Dark";

        public Dark()
        {
            InitializeComponent();
        }
    }
}
