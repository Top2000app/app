using Avalonia.Controls;
using Avalonia.Interactivity;
using Top2000.Apps.AvaloniaApp.ViewModels;

namespace Top2000.Apps.AvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ((MainWindowViewModel)DataContext!).InitialiseViewModelAsync();
    }

    public void MoveSelectionInFocus()
    {
        
        
    }
}