using System.Globalization;
using System.Reflection;
using Top2000.Data.Csv.Models;

namespace Top2000.Data.Csv;

public interface ITop2000DataProvider
{

}

public class Top2000DataProvider
{
    private const int ColumnCountBeforeEditions = 5;

    public async Task<Top2000DataContext> LoadFromAssemblyAsync()
    {
        var data = await GetDataFromAssemblyAsync();

        var context = new Top2000DataContext();
        var lines = data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var metadata = lines[0].Split(';');

        context.Editions =  metadata.Skip(ColumnCountBeforeEditions)
            .Select(x => new Edition { Year = int.Parse(x) })
            .ToList();

        for (var i = 1; i < lines.Length; i++)
        {
            var items = lines[i].Split(";");
            var id = int.Parse(items[0]);
            DateTime? lastPlayUtc = null;
            var firstEdition = -1;
            if (DateTime.TryParseExact(items[4], "dd-MM-yyyy HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out var playTime))
            {
                lastPlayUtc = playTime;
            }

            for (var l = ColumnCountBeforeEditions; l < metadata.Length; l++)
            {
                var position = items[l].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                var edition = metadata[l].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");

                if (!string.IsNullOrWhiteSpace(position))
                {
                    if (firstEdition == -1)
                    {
                        firstEdition = int.Parse(edition);
                    }

                    context.Listings.Add(new Listing
                    {
                        Edition = int.Parse(edition),
                        Position = int.Parse(position),
                        TrackId = id
                    });
                }
            }

            context.Tracks.Add(new Track
            {
                Id = id,
                Title = items[1],
                Artist = items[2],
                RecordedYear = int.Parse(items[3]),
                LastPlayUtc = lastPlayUtc,
                FirstEdition = firstEdition
            });
        }

        return context;
    }

    private async Task<string> GetDataFromAssemblyAsync()
    {
        using var csvFileStream = Assembly
            .GetAssembly(this.GetType())?
            .GetManifestResourceStream("top2000.csv")
            ?? throw new InvalidOperationException("Unable to find 'top2000.csv' in assembly");

        using var streamReader = new StreamReader(csvFileStream);
        return await streamReader.ReadToEndAsync();
    }
}
