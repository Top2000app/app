namespace Top2000.Data.ClientDatabase;

public class Top2000ServiceBuilder
{
    public string Directory { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
    public string Name {get; private set; } = "Top2000v2.db";
    public bool OnlineUpdatesEnabled { get; private set; } = true;
    public Uri UpdateUri { get; private set; } = new("https://data.top2000.app/");

    /// <summary>
    /// Specify the directory in which to create the SQLite database
    /// </summary>
    public Top2000ServiceBuilder DatabaseDirectory(string directory)
    {
        this.Directory = directory;
        return this;
    }

    /// <summary>
    /// Specify the name for the SQLite database
    /// </summary>
    /// <param name="name">Name of the database</param>
    public Top2000ServiceBuilder DatabaseName(string name)
    {
        this.Name = name;
        return this;
    }

    /// <summary>
    /// Enables the online update feature
    /// </summary>
    /// <returns></returns>
    public Top2000ServiceBuilder EnableOnlineUpdates(bool enabled = true)
    {
        this.OnlineUpdatesEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Enabled the online update feature with a custom Uri
    /// </summary>
    /// <param name="updateUri">Custom Uri</param>
    public Top2000ServiceBuilder EnableOnlineUpdates(Uri updateUri)
    {
        this.UpdateUri = updateUri;
        this.OnlineUpdatesEnabled = true;
        return this;
    }
}
