namespace Top2000MauiApp.Globalisation;

public interface ICulture
{
    string Name { get; }
}

public class SupportedCulture : ICulture
{
    public SupportedCulture(string name)
    {
        this.Name = name;
    }

    public string Name { get; }
}
