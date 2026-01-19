using Top2000.Features;
using Top2000.Features.Editions;
using Top2000.Features.Listings;
using Top2000.Features.Searching;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public class MockupTop2000Services : ITop2000Services
{
    public Task<HashSet<TrackListing>> AllListingsOfEditionAsync(int edition, CancellationToken cancellationToken = default)
    {
        var listings = new HashSet<TrackListing>
        {
            new()
            {
                TrackId = 1,
                Artist = "Artist A",
                Title = "Song A",
                Position = 1,
                PlayUtcDateAndTime = new DateTime(2025,
                    12,
                    25,
                    10,
                    0,
                    0),
                DeltaType = TrackListingDeltaType.NoChange
            },
            new()
            {
                TrackId = 2,
                Artist = "Artist B",
                Title = "Song B",
                Position = 2,
                PlayUtcDateAndTime = new DateTime(2025,
                    12,
                    25,
                    10,
                    5,
                    0),
                DeltaType = TrackListingDeltaType.Increased,
                Delta = 1289
            },
            new()
            {
                TrackId = 3,
                Artist = "Artist C",
                Title = "Song C",
                Position = 3,
                PlayUtcDateAndTime = new DateTime(2025, 12, 25, 10, 10, 0),
                DeltaType = TrackListingDeltaType.Decreased,
                Delta = -2
            },
            new()
            {
                TrackId = 4,
                Artist = "Artist C",
                Title = "This is a song with a very long title that might need to be truncated",
                Position = 4,
                PlayUtcDateAndTime = new DateTime(2025, 12, 25, 10, 10, 0),
                DeltaType = TrackListingDeltaType.New,
            },
            new()
            {
                TrackId = 5,
                Artist = "This is a long artist name that could potentially overflow the display area and might need truncation",
                Title = "Song E",
                Position = 4,
                PlayUtcDateAndTime = new DateTime(2025, 12, 25, 10, 10, 0),
                DeltaType = TrackListingDeltaType.Recurring,
            }
        };

        return Task.FromResult(listings);
    }

    public Task<TrackDetails> TrackDetailsAsync(int trackId, CancellationToken cancellationToken = default)
    {
        var edition = DateTime.Now.Year;
        var details = new TrackDetails
        {
            Artist = "The song was played by Artist A",
            Title = "The is some song Title that is nice to look at",
            RecordedYear = 2020,
            Listings =
            [
                new ListingInformation
                {
                    Edition = edition,
                    Position = 5,
                    PlayUtcDateAndTime = new DateTime(2023, 12, 25, 10, 0, 0),
                    Status = ListingStatus.New
                },

                new ListingInformation
                {
                    Edition = edition--,
                    Position = 3,
                    PlayUtcDateAndTime = new DateTime(2024, 12, 25, 10, 0, 0),
                    Status = ListingStatus.Back
                },

                new ListingInformation
                {
                    Edition = edition--,
                    Position = 4,
                    PlayUtcDateAndTime = new DateTime(2025, 12, 25, 10, 0, 0),
                    Status = ListingStatus.Decreased,
                    Delta = -1
                },
                new ListingInformation
                {
                    Edition = edition--,
                    Position = 1,
                    PlayUtcDateAndTime = new DateTime(2025, 12, 25, 10, 0, 0),
                    Status = ListingStatus.Increased,
                    Delta = 2053
                },
                new ListingInformation
                {
                    Edition = edition,
                    Position = 1,
                    PlayUtcDateAndTime = new DateTime(2025, 12, 25, 10, 0, 0),
                    Status = ListingStatus.Unchanged,
                },
            ]
        };

        return Task.FromResult(details);
    }

    public Task<SortedSet<Edition>> AllEditionsAsync(CancellationToken cancellationToken = default)
    {
        var set = new SortedSet<Edition>
        {
            new()
            {
                Year = 2025,
                StartUtcDateAndTime = new DateTime(2025, 12, 25),
                EndUtcDateAndTime = new DateTime(2026, 1, 1),
                HasPlayDateAndTime = true
            }
        };

        return Task.FromResult(set);
    }

    public Task<List<IGrouping<string, SearchedTrack>>> SearchAsync(string queryString, int latestYear, ISort sorting, IGroup group,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task InitialiseDataAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<int> DataVersion(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(42);
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}