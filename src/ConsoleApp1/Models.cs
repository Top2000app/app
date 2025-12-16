public class RootObject
{
    public Positions[] positions { get; set; }
}

public class Positions
{
    public string broadcastTime { get; set; }
    public string broadcastTimeFormatted { get; set; }
    public long broadcastUnixTime { get; set; }
    public Position position { get; set; }
    public Track track { get; set; }
}

public class Position
{
    public int current { get; set; }
    public string label { get; set; }
    public int previous { get; set; }
    public string type { get; set; }
}

public class Track
{
    public string artist { get; set; }
    public string title { get; set; }
    public int myId { get; set; }
}

