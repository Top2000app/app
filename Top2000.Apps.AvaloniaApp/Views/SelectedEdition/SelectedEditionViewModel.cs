using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Apps.AvaloniaApp.ViewModels;

namespace Top2000.Apps.AvaloniaApp.Views.SelectedEdition;

public partial class SelectedEditionViewModel : ViewModelBase
{
    [ObservableProperty] public int _year;
}

public class DesignTimeSelectedEditionViewModel : SelectedEditionViewModel
{
    public DesignTimeSelectedEditionViewModel()
    {
        Year = 2023;
    }
}