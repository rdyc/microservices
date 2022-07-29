using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Order.Orders.GettingOrderAtVersion;
using Order.Orders.GettingOrderById;
using Order.Orders.GettingOrderHistory;
using Order.Orders.GettingOrders;
using Swashbuckle.AspNetCore.Annotations;

namespace Order.WebApi.Endpoints;

public static class OrderEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all orders", OperationId = "orders", Tags = new[] { "Order" })]
    internal static async Task<IResult> Orders(
        [FromRoute] Guid clientId,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetOrders, IListPaged<OrderShortInfo>>(
            GetOrders.Create(clientId, page, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve an order", OperationId = "order", Tags = new[] { "Order" })]
    internal static async Task<IResult> Order(
        [FromRoute] Guid clientId,
        [FromRoute] Guid orderId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetOrderById, OrderDetails>(
            GetOrderById.Create(clientId, orderId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve order at version", OperationId = "order_version", Tags = new[] { "Order" })]
    internal static async Task<IResult> OrderAtVersion(
        [FromRoute] Guid clientId,
        [FromRoute] Guid orderId,
        [FromRoute] ulong version,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetOrderAtVersion, Orders.Order>(
            GetOrderAtVersion.Create(orderId, version), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve order histories", OperationId = "order_history", Tags = new[] { "Order" })]
    internal static async Task<IResult> Histories(
        [FromRoute] Guid clientId,
        [FromRoute] Guid orderId,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetOrderHistory, IListPaged<OrderHistory>>(
            GetOrderHistory.Create(orderId, page, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}