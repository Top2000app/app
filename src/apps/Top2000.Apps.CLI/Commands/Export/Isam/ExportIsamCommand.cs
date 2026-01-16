using System.Text;
using DownloaderApp;
using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Export.Isam;


public class ExportIsamCommand(Top2000Services _top2000Services, DownloaderApp.Database _database) : CommandBase("isam", "Export the DOS ISAM database for the Top2000")
{
    private const int PageSize = 512;
    private List<byte[]> editionsDataset;
    private List<byte[]> listingsDataset;
    private List<byte[]> tracksDataset;
    
    private List<TrackDbRecord> tracksDetails = [];
    private List<ListingDbRecord> listingsDetails = [];
    private List<EditionDbRecord> _editionsDetails = [];
    
    private Field[] tracksSchema = new[]
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
    
    private Field[] editionsSchema = new[]
    {
        new Field(FieldType.Int16, 2)
    };
    
    private Field[] listingsSchema = new[]
    {
        new Field(FieldType.Int16, 2),
        new Field(FieldType.Int16, 2),
        new Field(FieldType.Int16, 2),
        new Field(FieldType.Int16, 2),
        new Field(FieldType.Int16, 2)
    };
    
    private Field[] catalogSchema = new[]
    {
        new Field(FieldType.Text, 16),   // Dataset name (space padded)
        new Field(FieldType.Int16, 2),   // Dataset number (1-based)
        new Field(FieldType.Int16, 2),   // Header page number
        new Field(FieldType.Int16, 2),   // Record length
        new Field(FieldType.Int16, 2),   // Flags
        new Field(FieldType.Int16, 2),   // Index count
        new Field(FieldType.Int16, 2),   // Reserved
        new Field(FieldType.Int32, 4),   // Record count (LONG = 4 bytes)
        new Field(FieldType.Bytes, 32)   // Reserved (32 bytes)
    };
    
    private async Task LoadAllAsync()
    {
        _editionsDetails = (await _top2000Services.AllEditionsAsync(CancellationToken.None))
            .Select(x => new EditionDbRecord{ Year = x.Year} )
            .ToList();

        editionsDataset = LoadEditions(_editionsDetails);
        var trackIds = await _database.AllTrackIdsAsync();
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
        
        listingsDataset = LoadListings(listings);
        tracksDataset = LoadTracks(tracksDetails);
    }


    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        await LoadAllAsync();
        
        await Csv.MakeItAsync(_editionsDetails, tracksDetails, listingsDetails);
        
        await using var fs = File.Create("TOP2000.DAT");
        await using var bw = new BinaryWriter(fs);
        
        var catalogRecords = new List<byte[]>
        {
            CreateCatalogRecord("$CATALOG", 1, 1, 64, 1, 0, 4),
            CreateCatalogRecord("editions", 2, 1, 2, 1, 0, editionsDataset.Count),
            CreateCatalogRecord("listings", 3, 1, 10, 1, 0, listingsDataset.Count),
            CreateCatalogRecord("tracks", 4, 1, 151, 1, 0, tracksDataset.Count)
        };

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
        // DATASET: $CATALOG  (MUST BE FIRST)
        // ------------------------------------------------------------
        currentPage = WriteDataset(
            bw,
            "$CATALOG",
            catalogSchema,          // recordLength = 64
            catalogRecords,
            currentPage
        );

        // ------------------------------------------------------------
        // DATASET: editions
        // ------------------------------------------------------------
        currentPage = WriteDataset(
            bw,
            "editions",
            editionsSchema,
            editionsDataset,
            currentPage
        );

        // ------------------------------------------------------------
        // DATASET: listings
        // ------------------------------------------------------------
        currentPage = WriteDataset(
            bw,
            "listings",
            listingsSchema,
            listingsDataset,
            currentPage
        );

        // ------------------------------------------------------------
        // DATASET: tracks
        // ------------------------------------------------------------
        currentPage = WriteDataset(
            bw,
            "tracks",
            tracksSchema,
            tracksDataset,
            currentPage
        );

        Console.WriteLine("TOP2000.DAT created.");
    }
    
    
    static byte[] CreateCatalogRecord(
        string datasetName,
        short datasetNumber,
        short headerPage,
        short recordLength,
        short flags,
        short indexCount,
        int recordCount)
    {
        var b = new byte[64];

        WriteText(b, 0, 16, datasetName);
        WriteInt16(b, 16, datasetNumber);
        WriteInt16(b, 18, headerPage);
        WriteInt16(b, 20, recordLength);
        WriteInt16(b, 22, flags);
        WriteInt16(b, 24, indexCount);
        WriteInt16(b, 26, 0);
        WriteInt32(b, 28, recordCount);

        // Remaining 32 bytes are zero
        return b;
    }
    
    
    static void WriteInt32(byte[] b, int offset, int value)
    {
        b[offset]     = (byte)(value & 0xFF);
        b[offset + 1] = (byte)((value >> 8) & 0xFF);
        b[offset + 2] = (byte)((value >> 16) & 0xFF);
        b[offset + 3] = (byte)((value >> 24) & 0xFF);
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
            byte typeCode = f.Type switch
            {
                FieldType.Int16 => 1,
                FieldType.Int32 => 1,
                FieldType.Text => 2,
                FieldType.Bytes => 3,
                _ => 0
            };
            bw.Write(typeCode);
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


    private List<byte[]> LoadEditions(List<EditionDbRecord> editions)
    {
        var records = new List<byte[]>(editions.Count);

        foreach (var edition in editions)
        {
            // VB INTEGER = Int16
            var value = (short)edition.Year;

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
    enum FieldType { Int16, Int32, Text, Bytes }

    record Field(FieldType Type, int Length);
}