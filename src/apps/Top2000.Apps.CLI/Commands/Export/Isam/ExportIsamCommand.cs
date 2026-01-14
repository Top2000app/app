using System.Text;
using DownloaderApp;
using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Export.Isam;


public class ExportIsamCommand : ICommand<ExportCommands>
{
    private readonly Top2000Services _top2000Services;
    private readonly DownloaderApp.Database _database;
    private const int PageSize = 512;
    
    public ExportIsamCommand(Top2000Services top2000Services, DownloaderApp.Database database)
    {
        _top2000Services = top2000Services;
        _database = database;
    }
    
    public Command Create()
    {
        var command = new Command("isam", "Export the DOS ISAM database for the Top2000");

        command.SetAction(HandleIsamExportAsync);

        return command;
    }

    private async Task<int> HandleIsamExportAsync(ParseResult result, CancellationToken token)
    {
        await using var fs = File.Create("TOP2000.DAT");
        await using var bw = new BinaryWriter(fs);

        // ------------------------------------------------------------
        // PAGE 0 — FILE HEADER
        // ------------------------------------------------------------
        WriteEmptyPage(bw);

        // ------------------------------------------------------------
        // PAGE 1 — SYSTEM / CATALOG PAGE
        // ------------------------------------------------------------
        WriteEmptyPage(bw);

        var currentPage = 2;

        // ------------------------------------------------------------
        // DATASET: editions
        // ------------------------------------------------------------
        var editionsSchema = new[]
        {
            new Field(FieldType.Int16, 2)
        };

        currentPage = WriteDataset(
            bw,
            "editions",
            editionsSchema,
            await LoadEditionsAsync(),
            currentPage
        );
        
        // Retrieve all information
        var trackIds = await _database.AllTrackIdsAsync();
        List<TrackDbRecord> tracksDetails = [];
        var listings = new List<ListingDbRecord>();
        foreach (var trackId in trackIds)
        {
            var details = await _database.TrackDetailsAsync(trackId);
            tracksDetails.Add(TrackDbRecord.ToTrackDbRecord(trackId, details));
            
            var listingsForTrack = details.Listings
                .Where(x => x.Status != ListingStatus.NotAvailable && x.Status != ListingStatus.NotListed &&
                            x.Status != ListingStatus.Unknown)
                .OrderBy(x => x.Edition)
                .Select(x => new ListingDbRecord
                {
                    Edition = x.Edition,
                    Position = x.Position ?? throw new InvalidOperationException("Cannot be null"),
                    TrackId = trackId,
                    Offset = ListingDbRecord.ReadOffSet(x.Offset),
                    OffsetType = ListingDbRecord.ToChr(x.Status)
                })
                .ToList();
            
            listings.AddRange(listingsForTrack);
        }



        // ------------------------------------------------------------
        // DATASET: listings
        // ------------------------------------------------------------
        var listingsSchema = new[]
        {
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2)
        };

        currentPage = WriteDataset(
            bw,
            "listings",
            listingsSchema,
            LoadListings(listings),
            currentPage
        );

        // ------------------------------------------------------------
        // DATASET: tracks
        // ------------------------------------------------------------
        var tracksSchema = new[]
        {
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Text, 57),
            new Field(FieldType.Text, 44),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Text, 26),
            new Field(FieldType.Int16, 2),
            new Field(FieldType.Int16, 2)
        };

        currentPage = WriteDataset(
            bw,
            "tracks",
            tracksSchema,
            LoadTracks(tracksDetails),
            currentPage
        );

        Console.WriteLine("TOP2000.DAT created.");
        return 1;
    }
    
     // ============================================================
    // DATASET WRITER
    // ============================================================
    static int WriteDataset(
        BinaryWriter bw,
        string name,
        Field[] schema,
        List<byte[]> records,
        int startPage)
    {
        int recordSize = 0;
        foreach (var f in schema) recordSize += f.Length;

        // ----------------------------
        // DATASET HEADER (1 PAGE)
        // ----------------------------
        long headerPos = bw.BaseStream.Position;
        WriteDatasetHeader(
            bw,
            name,
            recordSize,
            schema,
            records.Count,
            startPage + 1,
            startPage + CountDataPages(records, recordSize)
        );

        int page = startPage + 1;

        // ----------------------------
        // DATA PAGES
        // ----------------------------
        int recIndex = 0;
        while (recIndex < records.Count)
        {
            var pageStart = bw.BaseStream.Position;
            bw.Write(new byte[8]); // data page header

            var slots = new List<ushort>();
            int offset = 8;

            while (recIndex < records.Count &&
                   offset + recordSize + (slots.Count + 1) * 2 <= PageSize)
            {
                bw.Write(records[recIndex]);
                slots.Add((ushort)offset);
                offset += recordSize;
                recIndex++;
            }

            // pad data area
            while (bw.BaseStream.Position < pageStart + PageSize - slots.Count * 2)
                bw.Write((byte)0);

            // slot directory
            foreach (var s in slots)
                WriteUInt16(bw, s);

            page++;
        }

        return page;
    }

    // ============================================================
    // HEADERS
    // ============================================================
    static void WriteDatasetHeader(
        BinaryWriter bw,
        string name,
        int recordSize,
        Field[] schema,
        int recordCount,
        int firstDataPage,
        int lastDataPage)
    {
        long start = bw.BaseStream.Position;

        WriteUInt16(bw, 0x5AA5);               // dataset marker
        WriteFixedString(bw, name, 16);        // dataset name
        WriteUInt16(bw, (ushort)recordSize);   // record length
        WriteUInt16(bw, (ushort)schema.Length);

        foreach (var f in schema)
        {
            bw.Write((byte)(f.Type == FieldType.Int16 ? 1 : 2));
            bw.Write((byte)0);
            WriteUInt16(bw, (ushort)f.Length);
        }

        WriteUInt32(bw, (uint)recordCount);
        WriteUInt32(bw, (uint)firstDataPage);
        WriteUInt32(bw, (uint)lastDataPage);
        WriteUInt16(bw, 0);                    // index count
        WriteUInt32(bw, 0);                    // index catalog ptr

        while (bw.BaseStream.Position < start + PageSize)
            bw.Write((byte)0);
    }

    // ============================================================
    // HELPERS
    // ============================================================
    static void WriteEmptyPage(BinaryWriter bw)
    {
        bw.Write(new byte[PageSize]);
    }

    static void WriteFixedString(BinaryWriter bw, string s, int len)
    {
        var b = Encoding.ASCII.GetBytes(s);
        for (int i = 0; i < len; i++)
            bw.Write(i < b.Length ? b[i] : (byte)' ');
    }

    static void WriteUInt16(BinaryWriter bw, ushort v)
    {
        bw.Write((byte)(v & 0xFF));
        bw.Write((byte)(v >> 8));
    }

    static void WriteUInt32(BinaryWriter bw, uint v)
    {
        bw.Write((byte)(v & 0xFF));
        bw.Write((byte)(v >> 8));
        bw.Write((byte)(v >> 16));
        bw.Write((byte)(v >> 24));
    }

    static int CountDataPages(List<byte[]> records, int recordSize)
    {
        int perPage = (PageSize - 8) / recordSize;
        return (records.Count + perPage - 1) / perPage;
    }


    private async Task<List<byte[]>> LoadEditionsAsync()
    {
        var editions = (await _top2000Services.AllEditionsAsync(CancellationToken.None))
            .Select(x => x.Year)
            .ToList();
        
        var records = new List<byte[]>(editions.Count);

        foreach (var edition in editions)
        {
            // VB INTEGER = Int16
            short value = (short)edition;

            var record = new byte[2];
            record[0] = (byte)(value & 0xFF);
            record[1] = (byte)((value >> 8) & 0xFF);

            records.Add(record);
        }

        return records;
    }

    private static List<byte[]> LoadListings(List<ListingDbRecord> listingDbRecords)
    {
        var records = new List<byte[]>(listingDbRecords.Count);

        foreach (var listing in listingDbRecords)
        {
            var record = new byte[10];

            WriteInt16(record, 0, (short)listing.Edition);
            WriteInt16(record, 2, (short)listing.Position);
            WriteInt16(record, 4, (short)listing.TrackId);
            WriteInt16(record, 6, (short)listing.Offset);
            WriteInt16(record, 8, (short)listing.OffsetType);

            records.Add(record);
        }

        return records;
    }

    static List<byte[]> LoadTracks(List<TrackDbRecord> tracks)
    {
        var records = new List<byte[]>(tracks.Count);

        foreach (var t in tracks)
        {
            var record = new byte[151];

            WriteInt16(record, 0,   t.TrackId);
            WriteText (record, 2,   57, t.Title);
            WriteText (record, 59,  44, t.Artist);
            WriteInt16(record, 103, t.RecordedYear);
            WriteInt16(record, 105, t.HighestPosition);
            WriteInt16(record, 107, t.HighestEdition);
            WriteInt16(record, 109, t.LowestPosition);
            WriteInt16(record, 111, t.LowestEdition);
            WriteInt16(record, 113, t.FirstPosition);
            WriteInt16(record, 115, t.FirstEdition);
            WriteInt16(record, 117, t.LatestPosition);
            WriteInt16(record, 119, t.LatestEdition);
            WriteText (record, 121, 26, t.LatestPlayLocalDateAndTime);
            WriteInt16(record, 147, t.Appearances);
            WriteInt16(record, 149, t.AppearancesPossible);

            records.Add(record);
        }

        return records;
    }


    static void WriteInt16(byte[] b, int o, short v)
    {
        b[o] = (byte)(v & 0xFF);
        b[o + 1] = (byte)(v >> 8);
    }

    static void WriteText(byte[] b, int o, int len, string s)
    {
        var t = Encoding.ASCII.GetBytes(s);
        for (int i = 0; i < len; i++)
            b[o + i] = i < t.Length ? t[i] : (byte)' ';
    }

    // ============================================================
    // STRUCTS
    // ============================================================
    enum FieldType { Int16, Text }

    record Field(FieldType Type, int Length);
}