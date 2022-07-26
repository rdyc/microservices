using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Payment.Payments.CompletingPayment;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.FailingPayment;
using Payment.Payments.FinalizingPayment;
using Payment.Payments.GettingPaymentById;
using Payment.Payments.GettingPayments;
using Payment.Payments.RequestingPayment;
using Payment.Payments.TimingOutPayment;

namespace Payment.Payments;

internal static class PaymentServices
{
    internal static IServiceCollection AddPayment(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Payment>, EventStoreDBRepository<Payment>>()
            .AddCommandHandlers()
            .AddCommandValidators()
            .AddQueryHandlers()
            .AddEventHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<RequestPayment, HandleRequestPayment>()
            .AddCommandHandler<CompletePayment, HandleCompletePayment>()
            .AddCommandHandler<DiscardPayment, HandleDiscardPayment>()
            .AddCommandHandler<TimeOutPayment, HandleTimeOutPayment>();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<RequestPayment, ValidateRequestPayment>()
            .AddCommandValidator<CompletePayment, ValidateCompletePayment>()
            .AddCommandValidator<DiscardPayment, ValidateDiscardPayment>()
            .AddCommandValidator<TimeOutPayment, ValidateTimeOutPayment>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetPayments, IListPaged<PaymentShortInfo>, HandleGetPayments>()
            .AddQueryHandler<GetPaymentById, PaymentDetails, HandleGetPaymentById>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<PaymentCompleted>, TransformIntoPaymentFinalized>()
            .AddEventHandler<EventEnvelope<PaymentDiscarded>, TransformIntoPaymentFailed>()
            .AddEventHandler<EventEnvelope<PaymentTimedOut>, TransformIntoPaymentFailed>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services
            .Projection<PaymentShortInfo>(builder => builder
                .AddOn<PaymentRequested>(PaymentShortInfoProjection.Handle)
                .UpdateOn<PaymentCompleted>(
                    onGet: e => e.PaymentId,
                    onHandle: PaymentShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<PaymentDiscarded>(
                    onGet: e => e.PaymentId,
                    onHandle: PaymentShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<PaymentTimedOut>(
                    onGet: e => e.PaymentId,
                    onHandle: PaymentShortInfoProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            )
            .Projection<PaymentDetails>(builder => builder
                .AddOn<PaymentRequested>(PaymentDetailsProjection.Handle)
                .UpdateOn<PaymentCompleted>(
                    onGet: e => e.PaymentId,
                    onHandle: PaymentDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.CompletedAt, view.CompletedAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<PaymentDiscarded>(
                    onGet: e => e.PaymentId,
                    onHandle: PaymentDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.DiscardedReason, view.DiscardedReason)
                        .Set(e => e.DiscardedAt, view.DiscardedAt)
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
                .UpdateOn<PaymentTimedOut>(
                    onGet: e => e.PaymentId,
                    onHandle: PaymentDetailsProjection.Handle,
                    onUpdate: (view, update) => update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.Position, view.Position)
                )
            );
}