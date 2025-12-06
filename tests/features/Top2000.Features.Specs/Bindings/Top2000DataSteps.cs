using Microsoft.Extensions.DependencyInjection;
using SQLite;
using Top2000.Data.ClientDatabase;
using Top2000.Data.ClientDatabase.Models;


namespace Top2000.Features.Specs.Bindings;

[Binding]
public class Top2000DataSteps
{
    private IGrouping<int, Listing> latestEdition;

    [BeforeScenario]
    public void DeleteClientDatabase()
    {
        if (File.Exists(App.DatabasePath))
        {
            File.Delete(App.DatabasePath);
        }
    }

    [AfterScenario]
    public void CloseDatabaseConnections()
    {
        SQLiteAsyncConnection.ResetPool();
    }

    [When(@"the client database is created")]
    [Given(@"the client database is created")]
    public async Task WhenTheClientDatabaseIsCreatedAsync()
    {
        var assemblySource = App.GetService<Top2000AssemblyDataSource>();
        var update = App.GetService<IUpdateClientDatabase>();

        await update.RunAsync(assemblySource);
    }

    [Then(@"the listing table contains 2000 tracks for each edition ranging from 1 to 2000")]
    public async Task ThenThePositionTableContainsTracksForEachEditionRangingFromTo()
    {
        var sql = App.GetService<SQLiteAsyncConnection>();
        var lists = (await sql.Table<Listing>().ToListAsync())
            .Where(x => x.Edition != 2023)
            .GroupBy(x => x.Edition)
            .OrderBy(x => x.Key)
            .ToList();

        var expected = Enumerable.Range(1, 2000);

        for (var i = 0; i < lists.Count -1 ; i++)
        {
            var yearPositions = lists[i].Select(x => x.Position).OrderBy(x => x);
            yearPositions.Should().BeEquivalentTo(expected);
        }
    }

    [Then(@"the listing table contains 10 or 2000 for the last edition ranging from 1 to 10/2000")]
    public async Task ThenThePositionsTableContainsOrForTheLastYearRangingFromTo()
    {
        var sql = App.GetService<SQLiteAsyncConnection>();
        var list = (await sql.Table<Listing>().Where(x => x.Edition != 2023).ToListAsync())
            .GroupBy(x => x.Edition)
            .OrderBy(x => x.Key)
            .Last();

        list.Count().Should().BeOneOf(10, 2000);
    }

    [Then(@"the listing table of edition 2023 has 2500 tracks")]
    public async Task ThenTheListingTableOfEditionHasTracks()
    {
        var sql = App.GetService<SQLiteAsyncConnection>();
        var list = (await sql.Table<Listing>().ToListAsync())
            .Where(x => x.Edition == 2023)
            .GroupBy(x => x.Edition)
            .OrderBy(x => x.Key)
            .Last();

        list.Count().Should().Be(2500);
    }

    [When("the latest edition is queried")]
    public async Task WhenTheLatestEditionIsQueried()
    {
        var sql = App.GetService<SQLiteAsyncConnection>();

        var lastEdition = (await sql.Table<Edition>().ToArrayAsync())
            .Last().Year;

        latestEdition = (await sql.Table<Listing>().ToListAsync())
            .Where(x => x.Edition == lastEdition)
            .GroupBy(x => x.Edition)
            .OrderBy(x => x.Key)
            .Last();
    }

    [Then("the latest edition contains either {int} or {int} items")]
    public void ThenTheLatestEditionContainsEitherOrOrItems(int p0, int p1)
    {
        latestEdition.Count().Should().BeOneOf(p0, p1);
    }

    [Then(@"for each track in the listing table the PlayDateAndTime is the same to the previous track or has incremented by one hour")]
    public async Task ThenForEachTrackInTheListingTableThePlayDateAndTimeIsTheSameToThePreviousTrackOrHasIncrementedByOneHour()
    {
        var sql = App.GetService<SQLiteAsyncConnection>();
        var listings = (await sql.Table<Listing>().Where(x => x.Edition > 2015).ToListAsync())
            .OrderBy(x => x.Position)
            .GroupBy(x => x.Edition)
            .OrderBy(x => x.Key)
            .ToList();

        foreach (var listing in listings)
        {
            var previous = listing.First();

            var listingForEdition = listing.Take(2000);

            foreach (var track in listingForEdition)
            {
                var differenceInHours = previous.PlayUtcDateAndTime - track.PlayUtcDateAndTime;

                Assert.IsTrue(differenceInHours.Value.TotalMinutes == 0 || differenceInHours.Value.TotalMinutes == 60,
                    $"For edition {listing.Key} the positions {previous.Position} and {track.Position} the PlayDateAndTime is incorrect, it is {differenceInHours.Value.TotalMinutes} and should be either 0 or 60"
                    );

                previous = track;
            }
        }
    }

    private class Listing
    {
        public int TrackId { get; set; }

        public int Edition { get; set; }

        public int Position { get; set; }

        public DateTime? PlayUtcDateAndTime { get; set; }
    }
}
