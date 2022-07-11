namespace Store.Currencies;

public record Currency
{
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string Symbol { get; private set; } = default!;
}