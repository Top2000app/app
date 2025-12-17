using Top2000.Features;
using Top2000.Features.Searching;
using Top2000MauiApp.Common;

namespace Top2000MauiApp.Pages.Searching;

public partial class ViewModel : ObservableObject
{
    private readonly Top2000Services _top2000Services;
    private readonly IEnumerable<IGroup> _groupOptions;
    private readonly IEnumerable<ISort> _sortOptions;

    public ViewModel(Top2000Services top2000Services, IEnumerable<IGroup> groupOptions, IEnumerable<ISort> sortOptions)
    {
        _top2000Services = top2000Services;
        _groupOptions = groupOptions;
        _sortOptions = sortOptions;
        GroupByOptions = [];
        SortByOptions = [];
        Results = [];
        ResultsFlat = [];
    }

    [ObservableProperty]
    public string? queryText;


    [ObservableProperty]
    public GroupViewModel? groupBy;


    [ObservableProperty]
    public SortViewModel? sortBy;


    [ObservableProperty]
    public bool isGrouped;


    [ObservableProperty]
    public bool isFlat;


    [ObservableProperty]
    public SearchedTrack? selectedTrack;

    [ObservableProperty]
    public string? resultsCount;

    public ObservableList<GroupViewModel> GroupByOptions { get; }

    public ObservableList<SortViewModel> SortByOptions { get; }

    public ObservableGroupedList<string, SearchedTrack> Results { get; }

    public ObservableList<SearchedTrack> ResultsFlat { get; }

    public void OnActivate(ISortGroupNameProvider nameProvider)
    {
        if (this.GroupByOptions.Count == 0)
        {
            this.GroupByOptions.ClearAddRange(_groupOptions.Select(x => new GroupViewModel(x, nameProvider.GetNameForGroup(x))));
            this.SortByOptions.ClearAddRange(_sortOptions.Select(x => new SortViewModel(x, nameProvider.GetNameForSort(x))));
            this.GroupBy = this.GroupByOptions[0];
            this.SortBy = this.SortByOptions[0];
        }
        else
        {
            foreach (var groupByOption in this.GroupByOptions)
            {
                groupByOption.Name = nameProvider.GetNameForGroup(groupByOption.Value);
            }

            foreach (var sortByOption in this.SortByOptions)
            {
                sortByOption.Name = nameProvider.GetNameForSort(sortByOption.Value);
            }
        }
    }

    public async Task ExceuteSearchAsync()
    {
        var editions = await _top2000Services.AllEditionsAsync();
        var lastEdition = editions.First();

        var result = await _top2000Services.SearchAsync(this.QueryText ?? "", lastEdition.Year, this.SortBy!.Value, this.GroupBy!.Value );

        this.Results.ClearAddRange(result);
        this.ResultsFlat.ClearAddRange(this.Results.SelectMany(x => x));

        this.ResultsCount = this.ResultsFlat.Count > 100
            ? "100+"
            : $"{this.ResultsFlat.Count}";
    }

    public void ReSortGroup()
    {
        if (this.GroupBy is null || this.SortBy is null)
        {
            return;
        }

        var resultFlat = this.Results.SelectMany(x => x).ToList();
        var sorted = this.SortBy.Value.Sort(resultFlat);
        this.ResultsFlat.ClearAddRange(sorted);

        var groupedAndSorted = this.GroupBy.Value.Group(sorted);
        this.Results.ClearAddRange(groupedAndSorted);

        this.IsGrouped = this.GroupBy.Value is not GroupByNothing;
        this.IsFlat = !this.IsGrouped;
    }
}
