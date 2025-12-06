using AwesomeAssertions;
using Top2000.Features.AllEditions;
using Top2000.Features.Searching;

namespace Top2000.Features.Specs.Bindings;

[Binding]
public class SearchSteps
{
    private List<IGrouping<string, SearchedTrack>> result = [];
    private readonly IGroup _grouping = new GroupByNothing();
    private readonly ISort _sorting = new SortByTitle();
    private readonly IMediator _mediator = App.GetService<IMediator>();

    [When(@"searching for (.*)")]
    public async Task WhenSearchingForAsync(string queryString)
    {
        var latestEdition = await GetLatestEditionAsync();
        var request = new SearchTrackRequest
        {
            QueryString = queryString,
            LatestYear = latestEdition,
            Sorting = _sorting,
            Grouping = _grouping,
        };

        result = await _mediator.Send(request);
    }

    [When(@"searching without a query")]
    public async Task WhenSearchingWithoutAQueryAsync()
    {
        var latestEdition = await GetLatestEditionAsync();
        var request = new SearchTrackRequest
        {
            QueryString = string.Empty,
            LatestYear = latestEdition,
            Sorting = _sorting,
            Grouping = _grouping,
        };

        result = await _mediator.Send(request);
    }

    [Then(@"the following tracks are found:")]
    public void ThenTracksAreFoundWithTheFollowingDetails(Table table)
    {
        var expected = table
            .CreateSet<SearchedTrack>()
            .Select(x => new TrackForAssertion
            {
                Id = x.Id,
                Title = x.Title,
                Artist = x.Artist,
                RecordedYear = x.RecordedYear,
            })
            .ToList();

        var actual = result
            .SelectMany(x => x)
            .Select(x => new TrackForAssertion
            {
                Id = x.Id,
                Title = x.Title,
                Artist = x.Artist,
                RecordedYear = x.RecordedYear,
            })
            .ToList();

        actual.Should().BeEquivalentTo(expected);
    }

    private async Task<int> GetLatestEditionAsync()
    {
        var allEditions = await _mediator.Send(new AllEditionsRequest());
        return allEditions.Last().Year;
    }

    private class TrackForAssertion
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Artist { get; set; } = string.Empty;

        public int RecordedYear { get; set; }
    }

    [Then(@"the track (.*) is not found")]
    public void ThenTrackIsNotFound(string title)
    {
        var actual = result
            .SelectMany(x => x)
            .Where(x => x.Title == title);

        actual.Should().BeEmpty();
    }

    [Then(@"the results contain (.*) items")]
    public void ThenTheResultsContainItems(int count)
    {
        var actual = result
            .SelectMany(x => x);

        actual.Should()
            .HaveCount(count);
    }
}
