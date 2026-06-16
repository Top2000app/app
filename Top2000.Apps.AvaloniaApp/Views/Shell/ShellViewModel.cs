using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Apps.AvaloniaApp.Views.TrackMenu;
using Top2000.Features;

namespace Top2000.Apps.AvaloniaApp.Views.Shell;

public partial class DesignTimeShellViewModel : ShellViewModel
{
    public DesignTimeShellViewModel()
        : base(new MockupTop2000Services(), new DesignTrackMenuViewModel())
    {
        
    }
}

public partial class ShellViewModel : ObservableObject, IShell
{
    private readonly ITop2000Services _top2000Services;
    [ObservableProperty]
    public partial bool IsLoading { get; set; } = true;

    [ObservableProperty]
    public partial string Title { get; set; } = "Top 2000";

    [ObservableProperty]
    public partial TrackMenuView? TrackMenuView { get; set; }

    public ShellViewModel(ITop2000Services top2000Services, TrackMenuViewModel trackMenuViewModel)
    {
        _top2000Services = top2000Services;
    }
    
    public async Task InitialiseAsync(TrackMenuViewModel trackMenuViewModel)
    {
        try
        {
            await Task.Delay(2000);
            await _top2000Services.InitialiseDataAsync();

            TrackMenuView = new TrackMenuView
            {
                DataContext = trackMenuViewModel
            };
            
            await trackMenuViewModel.InitialiseViewModelAsync(TrackMenuView, this);
            
            IsLoading = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}