using System.Collections.Generic;
using Top2000.Features.AllListingsOfEdition;

namespace Top2000.Features.Specs.Bindings;

[Binding]
public class AllListingsOfEditionSteps
{
    private HashSet<TrackListing> result;

    [Then(@"an empty set is returned")]
    public void ThenAnEmptySetIsReturned()
    {
        result.Should().BeEmpty();
    }

    [When(@"the AllListingOfEdition feature is executed with year (.*)")]
    public async Task WhenTheAllListingOfEditionFeatureIsExecutedWithYear(int year)
    {
        var request = new AllListingsOfEditionRequest { Year = year };
        var mediator = App.GetService<IMediator>();

        result = await mediator.Send(request);
    }

    [Then("the start of the list is {int}")]
    public void ThenTheStartOfTheListIs(int first)
    {
        result.First().Position.Should().Be(first);
    }

    [Then("the end of the list is {int}")]
    public void ThenTheEndOfTheListIs(int last)
    {
        result.Last().Position.Should().Be(last);
    }
}
