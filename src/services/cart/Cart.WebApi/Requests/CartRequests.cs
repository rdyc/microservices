namespace Cart.WebApi.Requests;

public record OpenCartRequest(
    Guid? ClientId
);

public record AddProductRequest(
    Guid? ProductId,
    int Quantity
);

public record RemoveProductRequest(
    Guid? ProductId,
    int? Quantity,
    decimal? UnitPrice
);

public record ConfirmCartRequest;