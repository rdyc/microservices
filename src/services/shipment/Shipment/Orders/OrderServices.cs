using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using Microsoft.Extensions.DependencyInjection;
using Shipment.Orders.GettingOrders;
using Shipment.Orders.RecordingOrderPayment;

namespace Shipment.Orders;

public static class OrderServices
{
    public static IServiceCollection AddOrder(this IServiceCollection services) =>
        services
            .AddQueryHandlers()
            .AddProjections();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetOrders, IListPaged<Order>, HandleGetOrders>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<Order>(builder => builder
                .AddOn<OrderPaymentRecorded>(OrderProjection.Handle)
            );
}