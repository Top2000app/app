using CommunityToolkit.Mvvm.Input;
using Top2000.Features;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Top2000Services _services;

    public MainWindowViewModel(Top2000Services services)
    {
        _services = services;
        Loaded();
    }
    
    public string Greeting { get; set; } = "Welcome to Avalonia!";

    public async void Loaded()
    {
        await _services.InitialiseDataAsync();
    }
}