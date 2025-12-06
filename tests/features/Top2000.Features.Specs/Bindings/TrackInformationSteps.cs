using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Top2000.Features.TrackInformation;


namespace Top2000.Features.Specs.Bindings;

[Binding]
public class TrackInformationSteps
{
    private TrackDetails track;

    [When(@"the track information feature is executed for TrackId (.*)")]
    public async Task WhenTheTrackInformationFeatureIsExecutedForTrackId(int trackId)
    {
        var request = new TrackInformationRequest { TrackId = trackId };

        var mediator = App.GetService<IMediator>();

        track = await mediator.Send(request);
    }

    [Then(@"the title is ""(.*)"" from '(.*)' which is recorded in the year (.*)")]
    public void ThenTheTitleIsFromWhichIsRecordedInTheYear(string title, string artist, int recordedYear)
    {
        using (new AssertionScope())
        {
            track.Title.Should().Be(title);
            track.Artist.Should().Be(artist);
            track.RecordedYear.Should().Be(recordedYear);
        }
    }

    [Then(@"the following years are listed as '(.*)'")]
    public void ThenTheFollowingYearsAreListedAs(string statusAsString, Table table)
    {
        var editions = table.CreateSet<EditionWithOffSet>();
        var status = (ListingStatus)Enum.Parse(typeof(ListingStatus), statusAsString);

        foreach (var edition in editions)
        {
            track.Listings.Single(x => x.Edition == edition.Edition).Status.Should().Be(status);
        }
    }

    [Then(@"the listing (.*) is listed as '(.*)'")]
    public void ThenTheListingIsListedAs(int edition, string statusAsString)
    {
        var status = (ListingStatus)Enum.Parse(typeof(ListingStatus), statusAsString);
        track.Listings.Single(x => x.Edition == edition).Status.Should().Be(status);
    }

    [Then(@"it could have been on the Top2000 for (.*) times")]
    public void ThenItCouldHaveBeenOnTheTopForTimes(int expected)
    {
        track.AppearancesPossible.Should().Be(expected);
    }

    [Then(@"is it listed for (.*) times")]
    public void ThenIsItListedForTimes(int expected)
    {
        track.Appearances.Should().Be(expected);
    }

    [Then(@"the record high is number (.*) on (.*)")]
    public void ThenTheRecordHighIsNumberOn(int position, int year)
    {
        using (new AssertionScope())
        {
            track.Highest.Position.Should().Be(position);
            track.Highest.Edition.Should().Be(year);
        }
    }

    [Then(@"the record low is number (.*) in (.*)")]
    public void ThenTheRecordLowIsNumberIn(int position, int year)
    {
        using (new AssertionScope())
        {
            track.Lowest.Position.Should().Be(position);
            track.Lowest.Edition.Should().Be(year);
        }
    }

    [Then(@"the Lastest position is number (.*) in (.*)")]
    public void ThenTheLastestPositionIsNumberIn(int position, int year)
    {
        using (new AssertionScope())
        {
            track.Latest.Position.Should().Be(position);
            track.Latest.Edition.Should().Be(year);
        }
    }

    [Then(@"the first position is number (.*) in (.*)")]
    public void ThenTheFirstPositionIsNumberIn(int position, int year)
    {
        using (new AssertionScope())
        {
            track.First.Position.Should().Be(position);
            track.First.Edition.Should().Be(year);
        }
    }

    private class EditionWithOffSet
    {
        public int Edition { get; set; }

        public int? Offset { get; set; }
    }
}
