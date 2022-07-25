namespace Shipment.Products;

public class Product
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}