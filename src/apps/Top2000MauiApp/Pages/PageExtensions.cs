namespace Top2000MauiApp.Pages;

public static class PageExtensions
{
    public static async Task<DisplayActionSheetResult> DisplayActionSheetAsync(this ContentPage page, string title, string cancel, params string[] options)
    {
        var result = await page.DisplayActionSheet(title, cancel, destruction: null, options);

        return new DisplayActionSheetResult
        (
            IsValid: result is not null && !result.Equals(cancel),
            SelectedOption: result ?? string.Empty
        );
    }
}

public readonly record struct DisplayActionSheetResult(bool IsValid, string SelectedOption);
