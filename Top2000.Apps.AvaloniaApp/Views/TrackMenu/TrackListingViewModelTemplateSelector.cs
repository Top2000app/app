using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class TrackListingViewModelTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<string, IDataTemplate> Templates {get;} = new();

    public Control? Build(object? param)
    {
        return Templates[((ITrackListingViewModel)param!).HeaderName].Build(param); 
    }

    public bool Match(object? data)
    {
        return data is ITrackListingViewModel;
    }
}