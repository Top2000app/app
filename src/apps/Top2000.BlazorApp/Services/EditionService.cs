namespace Top2000.BlazorApp.Services;

public class EditionService
{
    public async Task<int> GetLatestEditionAsync()
    {
        await Task.Yield();
        return 1;
    }
}