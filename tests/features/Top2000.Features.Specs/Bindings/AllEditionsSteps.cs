using Top2000.Features.Editions;

namespace Top2000.Features.Specs.Bindings;

[Binding]
public class AllEditionsSteps
{
    private SortedSet<Edition> _editions = [];

    [Given(@"All data scripts")]
    public Task GivenAllDataScriptsAsync()
    {
        var assemblySource = App.GetService<Top2000AssemblyDataSource>();
        var update = App.GetService<IUpdateClientDatabase>();

        return update.RunAsync(assemblySource);
    }

    [When(@"the feature is executed")]
    public async Task WhenTheFeatureIsExecutedAsync()
    {
        _editions = await App.Top2000Services().AllEditionsAsync();
    }

    [Then(@"a sorted set is returned started with the highest year")]
    public void ThenASortedSetIsReturnedStartedWithTheHighestYear()
    {
        var firstEdition = _editions.First();

        foreach (var edition in _editions)
        {
            edition.Year.Should().BeLessThanOrEqualTo(firstEdition.Year);
        }
    }

    [Then(@"the latest year is (.*)")]
    public void ThenTheLatestYearIs(int year)
    {
        var lastestEdition = _editions.Last();

        lastestEdition.Year.Should().Be(year);
    }

    [Then(@"the Start- and EndDateTime is in local time")]
    public void ThenTheStartAndEndDateTimeIsInLocalTime()
    {
        var offset = TimeZoneInfo.Local.BaseUtcOffset;

        foreach (var edition in _editions)
        {
            TimeZoneInfo.Local.GetUtcOffset(edition.LocalStartDateAndTime).Should().Be(offset);
            TimeZoneInfo.Local.GetUtcOffset(edition.LocalEndDateAndTime).Should().Be(offset);
        }
    }

    [Then(@"the UTC Start date is as follow:")]
    public void ThenTheUtcStartDateIsAsFollow(Table table)
    {
        var items = table.CreateSet<YearTimeCombo>();

        foreach (var item in items)
        {
            var edition = _editions.Single(x => x.Year == item.Year);

            edition.LocalStartDateAndTime.ToUniversalTime().Should().Be(item.UTCStartdate);
        }
    }

    private class YearTimeCombo
    {
        public int Year { get; set; }

        public DateTime UTCStartdate { get; set; }
    }
}
