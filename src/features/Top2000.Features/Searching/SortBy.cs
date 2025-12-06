namespace Top2000.Features.Searching;

public static class SortBy
{
    private static readonly ISort defaultSort = new SortByTitle();

    public static ISort Default => defaultSort;
}
