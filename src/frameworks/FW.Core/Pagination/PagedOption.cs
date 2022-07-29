namespace FW.Core.Pagination;

public record PagedOption
{
    private PagedOption(int page, int size)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (size < 1 || size > 100) throw new ArgumentOutOfRangeException(nameof(size));

        Page = page;
        Size = size;
    }

    public int Page { get; }
    public int Size { get; }

    public static PagedOption Create(int page, int size) =>
        new(page, size);

    public void Deconstruct(out int page, out int size)
    {
        page = Page;
        size = Size;
    }
}