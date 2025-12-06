namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class SqlScriptTest
{
    [TestMethod]
    public void ScriptSectionReturnsContentSplitOnSemicolon()
    {
        const string contents = "Statement1;Statement2";

        var sut = new SqlScript("TheName", contents);

        var actual = sut.SqlSections();
        var expected = new[] { "Statement1", "Statement2" };

        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void EmptySectionAreRemoved()
    {
        const string contents = "Statement1;    ;Statement2;";

        var sut = new SqlScript("TheName", contents);

        var actual = sut.SqlSections();
        var expected = new[] { "Statement1", "Statement2" };

        actual.Should().BeEquivalentTo(expected);
    }
}
