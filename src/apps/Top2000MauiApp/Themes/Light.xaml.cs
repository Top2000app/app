
using Microsoft.Maui.Controls.Xaml;

namespace Top2000MauiApp.Themes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Light : ResourceDictionary
    {
        public const string ThemeName = "Light";

        public Light()
        {
            InitializeComponent();
        }
    }
}
