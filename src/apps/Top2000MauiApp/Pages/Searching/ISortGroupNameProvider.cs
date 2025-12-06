using Top2000.Features.Searching;

namespace Top2000MauiApp.Pages.Searching;

public interface ISortGroupNameProvider
{
    string GetNameForGroup(IGroup group);

    string GetNameForSort(ISort sort);
}