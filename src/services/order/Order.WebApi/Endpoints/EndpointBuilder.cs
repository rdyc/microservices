using System.Net;
using FW.Core.Pagination;
using Order.Orders.GettingOrderById;
using Order.Orders.GettingOrderHistory;
using Order.Orders.GettingOrders;

namespace Order.WebApi.Endpoints;

internal static class EndpointBuilder
{
    internal static IApplicationBuilder UseOrderEndpoints(this WebApplication app) =>
        app.UseOrderEndpoint();

    private static WebApplication UseOrderEndpoint(this WebApplication app)
    {
        app.MapGet("/orders/{clientId}", OrderEndpoint.Orders).Produces((int)HttpStatusCode.OK, typeof(IListPaged<OrderShortInfo>));
        app.MapGet("/orders/{clientId}/{orderId}", OrderEndpoint.Order).Produces((int)HttpStatusCode.OK, typeof(OrderDetails));
        app.MapGet("/orders/{clientId}/{orderId}/version/{version}", OrderEndpoint.OrderAtVersion).Produces((int)HttpStatusCode.OK, typeof(Orders.Order));
        app.MapGet("/orders/{clientId}/{orderId}/histories", OrderEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<OrderHistory>));

        return app;
    }
}