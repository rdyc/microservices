using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.GettingOrderAtVersion;
using Order.Orders.GettingOrderById;
using Order.Orders.GettingOrderHistory;
using Order.Orders.GettingOrders;
using Order.Orders.InitializingOrder;
using Order.Orders.ProcessingOrder;
using Order.Orders.RecordingOrderPayment;
using Order.Payments.FinalizingPayment;
using Order.Payments.TimingOutPayment;
using Order.Shipments.DiscardingPackage;
using Order.Shipments.RequestingPackage;
using Order.Shipments.SendingPackage;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders;

internal static class OrderServices
{
    internal static IServiceCollection AddOrder(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Order>, EventStoreDBRepository<Order>>()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddEventHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<InitializeOrder, HandleInitializeOrder>()
            .AddCommandHandler<RecordOrderPayment, HandleRecordOrderPayment>()
            .AddCommandHandler<ProcessOrder, HandleProcessOrder>()
            .AddCommandHandler<CompleteOrder, HandleCompleteOrder>()
            .AddCommandHandler<CancelOrder, HandleCancelOrder>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetOrderById, OrderDetails, HandleGetOrderById>()
            .AddQueryHandler<GetOrders, IListPaged<OrderShortInfo>, HandleGetOrders>()
            .AddQueryHandler<GetOrderHistory, IListPaged<OrderHistory>, HandleGetOrderHistory>()
            .AddQueryHandler<GetOrderAtVersion, Order, HandleGetOrderAtVersion>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<ShoppingCartFinalized, OrderSaga>()
            .AddEventHandler<OrderInitialized, OrderSaga>()
            .AddEventHandler<PaymentFinalized, OrderSaga>()
            .AddEventHandler<PaymentTimedOut, OrderSaga>()
            .AddEventHandler<PackagePrepared, OrderSaga>()
            .AddEventHandler<PackageWasSent, OrderSaga>()
            .AddEventHandler<ProductWasOutOfStock, OrderSaga>()
            .AddEventHandler<OrderCancelled, OrderSaga>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<OrderShortInfo>(builder => builder
                .AddOn<OrderInitialized>(OrderShortInfoProjection.Handle)
                .UpdateOn<OrderPaymentRecorded>(
                    onGet: e => e.OrderId,
                    onHandle: OrderShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<OrderCancelled>(
                    onGet: e => e.OrderId,
                    onHandle: OrderShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<OrderCompleted>(
                    onGet: e => e.OrderId,
                    onHandle: OrderShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<OrderDetails>(builder => builder
                .AddOn<OrderInitialized>(OrderDetailsProjection.Handle)
                .UpdateOn<OrderPaymentRecorded>(
                    onGet: e => e.OrderId,
                    onHandle: OrderDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.PaymentId, view.PaymentId)
                        .Set(e => e.PaidAt, view.PaidAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<OrderCancelled>(
                    onGet: e => e.OrderId,
                    onHandle: OrderDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.CancellationReason, view.CancellationReason)
                        .Set(e => e.CancelledAt, view.CancelledAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<OrderCompleted>(
                    onGet: e => e.OrderId,
                    onHandle: OrderDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.CompletedAt, view.CancelledAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<OrderHistory>(builder => builder
                .AddOn<OrderInitialized>(OrderHistoryProjection.Handle)
                .AddOn<OrderPaymentRecorded>(OrderHistoryProjection.Handle)
                .AddOn<OrderCancelled>(OrderHistoryProjection.Handle)
                .AddOn<OrderCompleted>(OrderHistoryProjection.Handle)
            );
}