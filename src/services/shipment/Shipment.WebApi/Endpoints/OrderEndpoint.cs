using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Shipment.Orders.GettingOrders;
using Swashbuckle.AspNetCore.Annotations;

namespace Shipment.WebApi.Endpoints;

public static class OrderEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all orders", OperationId = "orders", Tags = new[] { "Order" })]
    internal static async Task<IResult> Orders(
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetOrders, IListPaged<Order>>(
            GetOrders.Create(index, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}