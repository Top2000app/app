using System.Collections.Immutable;
using Top2000.Data.ClientDatabase;
using Top2000.Features.AllEditions;

namespace Top2000.Features.Specs.Bindings;

[Binding]
public class AllEditionsSteps
{
    private SortedSet<Edition> editions;

    [Given(@"All data scripts")]
    public Task GivenAllDataScripts()
    {
        var assemblySource = App.GetService<Top2000AssemblyDataSource>();
        var update = App.GetService<IUpdateClientDatabase>();

        return update.RunAsync(assemblySource);
    }

    [When(@"the feature is executed")]
    public async Task WhenTheFeatureIsExecuted()
    {
        var request = new AllEditionsRequest();

        var mediator = App.GetService<IMediator>();

        editions = await mediator.Send(request);
    }

    [Then(@"a sorted set is returned started with the highest year")]
    public void ThenASortedSetIsReturnedStartedWithTheHighestYear()
    {
        var firstEdition = editions.First();

        foreach (var edition in editions)
        {
            edition.Year.Should().BeLessThanOrEqualTo(firstEdition.Year);
        }
    }

    [Then(@"the latest year is (.*)")]
    public void ThenTheLatestYearIs(int year)
    {
        var lastestEdition = editions.Last();

        lastestEdition.Year.Should().Be(year);
    }

    [Then(@"the Start- and EndDateTime is in local time")]
    public void ThenTheStart_AndEndDateTimeIsInLocalTime()
    {
        var offset = TimeZoneInfo.Local.BaseUtcOffset;

        foreach (var edition in editions)
        {
            TimeZoneInfo.Local.GetUtcOffset(edition.LocalStartDateAndTime).Should().Be(offset);
            TimeZoneInfo.Local.GetUtcOffset(edition.LocalEndDateAndTime).Should().Be(offset);
        }
    }

    [Then(@"the UTC Start date is as follow:")]
    public void Then_TheUTCStartDateIsAsFollow(Table table)
    {
        var items = table.CreateSet<YearTimeCombo>();

        foreach (var item in items)
        {
            var edition = editions.Single(x => x.Year == item.Year);

            edition.LocalStartDateAndTime.ToUniversalTime().Should().Be(item.UTCStartdate);
        }
    }

    private class YearTimeCombo
    {
        public int Year { get; set; }

        public DateTime UTCStartdate { get; set; }
    }
}
