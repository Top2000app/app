using Microsoft.Extensions.Options;

namespace Top2000MauiApp.AskForReview;

public interface IStoreReview
{
    void OpenStoreReviewPage(string appId);
}

public class AskForReviewConfiguration
{
    public bool AskForReview { get; set; }

    public string AppId { get; set; } = string.Empty;
}

public interface IAskForReview
{
    void AskForReview();

    bool MustAskForReview();
}

public class ReviewModule : IAskForReview
{
    private static readonly int[] AskForReviews = [8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597];
    private readonly IOptions<AskForReviewConfiguration> _configuration;
    private readonly IPreferences _preferences;
    private readonly IStoreReview _storeReview;

    public ReviewModule(IOptions<AskForReviewConfiguration> configuration, IPreferences preferences, IStoreReview storeReview)
    {
        this._configuration = configuration;
        this._preferences = preferences;
        this._storeReview = storeReview;
    }

    public int StartupCountForReview
    {
        get { return _preferences.Get(nameof(this.StartupCountForReview), 0); }
        set { _preferences.Set(nameof(this.StartupCountForReview), value); }
    }

    private bool HasReviewed
    {
        get { return _preferences.Get(nameof(this.HasReviewed), false); }
        set { _preferences.Set(nameof(this.HasReviewed), value); }
    }

    public bool MustAskForReview()
    {
        if (!_configuration.Value.AskForReview || this.HasReviewed)
        {
            return false;
        }

        var count = this.StartupCountForReview++;

        return AskForReviews.Contains(count);
    }

    public void AskForReview()
    {
        this.HasReviewed = true;
        _storeReview.OpenStoreReviewPage(_configuration.Value.AppId);
    }
}
