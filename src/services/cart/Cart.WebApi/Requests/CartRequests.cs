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
    decimal? Price
);

public record ConfirmCartRequest;