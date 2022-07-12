namespace Store.WebApi.Requests;

public record ProductCreateRequest(string SKU, string Name, string Description);
public record ProductModifyRequest(string SKU, string Name, string Description);
public record AddProductAttributeRequest(Guid AttributeId, string Value);
public record UpdateProductPriceRequest(Guid CurrencyId, decimal Price);
public record UpdateProductStockRequest(int Stock);