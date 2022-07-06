namespace FW.Core.Pagination;

public interface IListUnpaged<T>
    where T : class
{
    IEnumerable<T> Items { get; }
}

public record ListUnpaged<T> : IListUnpaged<T>
    where T : class
{
    private ListUnpaged(IEnumerable<T> items)
    {
        Items = items;
    }

    public static IListUnpaged<T> Create(IEnumerable<T> items) =>
        new ListUnpaged<T>(items);

    public IEnumerable<T> Items { get; init; } = default!;
}

public interface IListPaged<T>
    where T : class
{
    IEnumerable<T> Items { get; }
    IMetadata Metadata { get; }
}

public record ListPaged<T> : IListPaged<T>
    where T : class
{
    private ListPaged(IEnumerable<T> items, IMetadata metadata)
    {
        Items = items;
        Metadata = metadata;
    }

    public IEnumerable<T> Items { get; init; } = default!;
    public IMetadata Metadata { get; init; } = default!;

    public static IListPaged<T> Create(IEnumerable<T> items, IMetadata metadata) =>
        new ListPaged<T>(items, metadata);
}

public interface IMetadata
{
    long Total { get; }
    IPage Page { get; }
}

public record Metadata : IMetadata
{
    private Metadata(long count, int index, int size)
    {
        Total = count;
        Page = Pagination.Page.Create(count, index, size);
    }

    public long Total { get; init; }
    public IPage Page { get; init; }

    public static IMetadata Create(long count, int index, int size) =>
        new Metadata(count, index, size);
}

public interface IPage
{
    int Size { get; }
    int Number { get; }
    int Count { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
    bool IsFirst { get; }
    bool IsLast { get; }
}

public record Page : IPage
{
    private Page(long count, int index, int size)
    {
        Size = size;
        Number = index + 1;
        Count = (int)Math.Ceiling((double)count / size);
        HasPrevious = Number > 1 && Number <= Count;
        HasNext = Number < Count && Number >= 1;
        IsFirst = Number == 1;
        IsLast = Number == Count;
    }

    public int Size { get; init; }
    public int Number { get; init; }
    public int Count { get; init; }
    public bool HasPrevious { get; init; }
    public bool HasNext { get; init; }
    public bool IsFirst { get; init; }
    public bool IsLast { get; init; }

    public static IPage Create(long count, int index, int size) =>
        new Page(count, index, size);
}