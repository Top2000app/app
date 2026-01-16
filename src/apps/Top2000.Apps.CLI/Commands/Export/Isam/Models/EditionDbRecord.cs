namespace Top2000.Apps.CLI.Commands.Export.Isam;

public class EditionDbRecord
{
    public required int Year { get; init; }

    public string ToCsvString()
    {
        return $"{Year}";
    }
}