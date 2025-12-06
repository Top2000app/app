using Top2000.Features.Searching;

namespace Top2000MauiApp.Pages.Searching;

public class SortViewModel
{
    public SortViewModel(ISort value, string name)
    {
        Value = value;
        Name = name;
    }

    public ISort Value { get; }

    public string Name { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is SortViewModel svm && svm.Name == Name;
    }

    public override int GetHashCode() => Name.GetHashCode();
}
