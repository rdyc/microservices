namespace Store.Products.RemovingAttribute;

public record AttributeRemoved(
    Guid Id,
    ProductAttribute ProductAttribute
)
{
    public static AttributeRemoved Create(Guid id, ProductAttribute productAttribute)
        => new(id, productAttribute);
}