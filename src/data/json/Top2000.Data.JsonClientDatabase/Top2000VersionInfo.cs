using Top2000.Data.JsonClientDatabase.Models;

namespace Top2000.Data.JsonClientDatabase;

public class Top2000VersionInfo
{
    public required int Version { get; init; }
    
    public required List<Edition> Editions { get; init; }
}