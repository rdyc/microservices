namespace Cart.WebApi.Requests;

public record OpenShoppingCartRequest(
    Guid? ClientId
);

public record ProductRequest(
    Guid? ProductId,
    int? Quantity
);

public record AddProductRequest(
    ProductRequest? Product
);

public record PricedProductRequest(
    Guid? ProductId,
    int? Quantity,
    decimal? UnitPrice
);

public record RemoveProductRequest(
    PricedProductRequest? Product
);

public record ConfirmShoppingCartRequest;