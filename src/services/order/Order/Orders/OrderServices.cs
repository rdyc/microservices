using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Order.Orders.CancellingOrder;
using Order.Orders.CompletingOrder;
using Order.Orders.GettingOrders;
using Order.Orders.InitializingOrder;
using Order.Orders.RecordingOrderPayment;
using Order.Payments.FinalizingPayment;
using Order.Shipments.OutOfStockProduct;
using Order.Shipments.SendingPackage;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders;

internal static class OrderServices
{
    internal static IServiceCollection AddOrder(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Order>, EventStoreDBRepository<Order>>()
            .AddCommandHandlers()
            .AddEventHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<InitializeOrder, HandleInitializeOrder>()
            .AddCommandHandler<RecordOrderPayment, HandleRecordOrderPayment>()
            .AddCommandHandler<CompleteOrder, HandleCompleteOrder>()
            .AddCommandHandler<CancelOrder, HandleCancelOrder>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<ShoppingCartFinalized, OrderSaga>()
            .AddEventHandler<PaymentFinalized, OrderSaga>()
            .AddEventHandler<PackageWasSent, OrderSaga>()
            .AddEventHandler<ProductWasOutOfStock, OrderSaga>()
            .AddEventHandler<OrderCancelled, OrderSaga>()
            .AddEventHandler<OrderPaymentRecorded, OrderSaga>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<OrderShortInfo>(builder => builder
                .AddOn<OrderInitialized>(OrderShortInfoProjection.Handle)
                .UpdateOn<OrderPaymentRecorded>(
                    onGet: e => e.OrderId,
                    onHandle: OrderShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.PaymentId, view.PaymentId)
                        .Set(e => e.PaidAt, view.PaidAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<OrderCancelled>(
                    onGet: e => e.OrderId,
                    onHandle: OrderShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.CancellationReason, view.CancellationReason)
                        .Set(e => e.CancelledAt, view.CancelledAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<OrderCompleted>(
                    onGet: e => e.OrderId,
                    onHandle: OrderShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.CompletedAt, view.CancelledAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            );
}