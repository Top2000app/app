namespace Top2000.Data.LocalDb;

public static class DbUpExtensions
{
    public static UpgradeEngineBuilder WithScriptEmbeddedInDataLibrary(this UpgradeEngineBuilder builder)
        => builder.WithScripts(new Top2000DataScriptProvider(new Top2000Data()));
}
