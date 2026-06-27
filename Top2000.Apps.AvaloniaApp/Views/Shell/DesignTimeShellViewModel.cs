using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Apps.AvaloniaApp.Views.TrackMenu;

namespace Top2000.Apps.AvaloniaApp.Views.Shell;

public partial class DesignTimeShellViewModel : ShellViewModel
{
    public DesignTimeShellViewModel()
        : base(new MockupTop2000Services())
    {
        
    }
}