using Top2000.Features.Searching;

namespace Top2000MauiApp.Pages.Searching;

public class GroupViewModel
{
    public GroupViewModel(IGroup value, string name)
    {
        this.Value = value;
        this.Name = name;
    }

    public IGroup Value { get; }

    public string Name { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        return obj is GroupViewModel svm && svm.Name == this.Name;
    }

    public override int GetHashCode() => this.Name.GetHashCode();
}
