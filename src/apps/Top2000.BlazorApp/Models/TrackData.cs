using System.Text.Json.Serialization;
using Top2000.Features.Listing;
using Top2000.Features.TrackInformation;

namespace Top2000.BlazorApp.Models;

public class TrackData
{
    [JsonPropertyName("trackId")]
    public int TrackId { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("delta")]
    public int Delta { get; set; }

    [JsonPropertyName("isRecurring")]
    public bool IsRecurring { get; set; }

    [JsonPropertyName("playUtcDateAndTime")]
    public DateTime? PlayUtcDateAndTime { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public string Artist { get; set; } = string.Empty;

    public ListingInformation ToListingInformation(int edition)
    {
        return new ListingInformation
        {
            Edition = edition,
            Position = Position,
            PlayUtcDateAndTime = PlayUtcDateAndTime,
            Offset = Delta,
            Status = ListingStatus.Unknown // Default status, will be calculated later if needed
        };
    }

    public TrackListing ToTrackListing()
    {
        return new TrackListing
        {
            TrackId = TrackId,
            Position = Position,
            Delta = Delta,
            IsRecurring = IsRecurring,
            PlayUtcDateAndTime = PlayUtcDateAndTime ?? DateTime.MinValue,
            Title = Title,
            Artist = Artist
        };
    }
}
