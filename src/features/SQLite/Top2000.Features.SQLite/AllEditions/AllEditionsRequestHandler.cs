using Top2000.Features.AllEditions;

namespace Top2000.Features.SQLite.AllEditions;

public class AllEditionsRequestHandler : IRequestHandler<AllEditionsRequest, SortedSet<Edition>>
{
    private readonly SQLiteAsyncConnection connection;

    public AllEditionsRequestHandler(SQLiteAsyncConnection connection)
    {
        this.connection = connection;
    }

    public async Task<SortedSet<Edition>> Handle(AllEditionsRequest request, CancellationToken cancellationToken)
    {
        var result = await connection
            .Table<Edition>()
            .ToListAsync();

        return new SortedSet<Edition>(result, new EditionDescendingComparer());
    }
}
