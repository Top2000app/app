using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Features;

namespace Top2000.Apps.AvaloniaApp.Views.Shell;

public interface IShell
{
    public string Title { get; }
}

public partial class ShellViewModel : ViewModelBase, IShell
{
    private readonly ITop2000Services _top2000Services;
    private readonly MainWindowViewModel _mainWindowViewModel;
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _loadingMessage = "Loading database...";
    [ObservableProperty] private string _title = "Top 2000";
    [ObservableProperty] private MainWindow? _mainWindow;

//    public ShellViewModel()
  //  {
    //    _top2000Services = new MockupTop2000Services();
    //}
    
    public ShellViewModel(ITop2000Services top2000Services, MainWindowViewModel  mainWindowViewModel)
    {
        _top2000Services = top2000Services;
        _mainWindowViewModel = mainWindowViewModel;
    }
    
    public async Task InitialiseAsync()
    {
        try
        {
            await _top2000Services.InitialiseDataAsync();

            MainWindow = new MainWindow
            {
                DataContext = _mainWindowViewModel
            };
            
            await _mainWindowViewModel.InitialiseViewModelAsync(MainWindow);
            
            IsLoading = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}