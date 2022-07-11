namespace Store.Attributes;

public class Attribute
{
    public Guid Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public AttributeType Type { get; private set; } = default!;
    public string Unit { get; private set; } = default!;
}