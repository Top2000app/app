namespace Top2000.Features.Searching;

public static class GroupBy
{
    private static readonly IGroup DefaultGroup = new GroupByNothing();

    public static IGroup Default => DefaultGroup;
}
