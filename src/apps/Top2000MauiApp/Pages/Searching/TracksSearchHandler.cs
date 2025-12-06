namespace Top2000MauiApp.Pages.Searching;

public class TracksSearchHandler : SearchHandler
{
    private ViewModel ViewModel => (ViewModel)BindingContext;

    protected override async void OnQueryChanged(string oldValue, string newValue)
    {
        if (ViewModel != null && newValue != null && newValue != oldValue)
        {
            ViewModel.QueryText = newValue;
            await ViewModel.ExceuteSearchAsync();
        }
    }
}
