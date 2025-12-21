using Top2000.Features.Listings;

namespace Top2000.Features.Specs.Bindings;

[Binding]
public class AllListingsOfEditionSteps
{
    private HashSet<TrackListing> _result = [];

    [Then(@"an empty set is returned")]
    public void ThenAnEmptySetIsReturned()
    {
        _result.Should().BeEmpty();
    }

    [When(@"the AllListingOfEdition feature is executed with year (.*)")]
    public async Task WhenTheAllListingOfEditionFeatureIsExecutedWithYearAsync(int year)
    {
        _result = await App.Top2000Services.AllListingsOfEditionAsync(year);
    }

    [Then("the start of the list is {int}")]
    public void ThenTheStartOfTheListIs(int first)
    {
        _result.First().Position.Should().Be(first);
    }

    [Then("the end of the list is {int}")]
    public void ThenTheEndOfTheListIs(int last)
    {
        _result.Last().Position.Should().Be(last);
    }
}
