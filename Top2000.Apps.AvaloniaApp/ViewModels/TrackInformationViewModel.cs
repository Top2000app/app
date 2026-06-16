using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Features;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public class TrackInformationViewModel : ObservableObject
{
    private readonly ITop2000Services _top2000Services;

    public TrackInformationViewModel()
    {
        _top2000Services = new MockupTop2000Services();
    }

    public TrackInformationViewModel(ITop2000Services top2000Services)
    {
        _top2000Services = top2000Services;
    }
    
    
}