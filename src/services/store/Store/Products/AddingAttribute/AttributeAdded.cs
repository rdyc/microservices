namespace Store.Products.AddingAttribute;

public record AttributeAdded(
    Guid Id,
    ProductAttribute ProductAttribute
)
{
    public static AttributeAdded Create(Guid id, ProductAttribute productAttribute)
        => new(id, productAttribute);
}