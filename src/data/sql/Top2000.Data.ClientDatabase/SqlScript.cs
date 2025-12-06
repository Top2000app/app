namespace Top2000.Data.ClientDatabase;

public class SqlScript
{
    private const char OnSemicolon = ';';

    public SqlScript(string scriptName, string contents)
    {
        ScriptName = scriptName;
        Contents = contents;
    }

    public string ScriptName { get; set; }

    public string Contents { get; }

    public string[] SqlSections()
    {
        return Contents
            .Split(OnSemicolon)
            .Where(HasContent)
            .ToArray();
    }

    private static bool HasContent(string x) => !string.IsNullOrWhiteSpace(x);
}
