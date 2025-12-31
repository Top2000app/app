using Top2000.Data.JsonClientDatabase.Models;

namespace Top2000.Features.Json.Data;

public class DataProvider
{
    public Top2000DataContext Value { get; private set; }

    public void SetValue(Top2000DataContext dataContext)
    {
        Value = dataContext;
    }
}