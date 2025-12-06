namespace Top2000.Features.Searching;

public static class SortBy
{
    private static readonly ISort DefaultSort = new SortByTitle();

    public static ISort Default => DefaultSort;
}
