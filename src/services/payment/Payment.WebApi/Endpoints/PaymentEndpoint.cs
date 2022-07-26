using FW.Core.Commands;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Payment.Payments.CompletingPayment;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.GettingPaymentById;
using Payment.Payments.GettingPayments;
using Payment.Payments.RequestingPayment;
using Payment.Payments.TimingOutPayment;
using Payment.WebApi.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Payment.WebApi.Endpoints;

public static class PaymentEndpoint
{
    [SwaggerOperation(Summary = "Retrieve all payments", OperationId = "payments", Tags = new[] { "Payment" })]
    internal static async Task<IResult> Payments(
        int index,
        int size,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetPayments, IListPaged<PaymentShortInfo>>(
            GetPayments.Create(index, size), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Retrieve payment", OperationId = "payment", Tags = new[] { "Payment" })]
    internal static async Task<IResult> Payment(
        [FromRoute] Guid paymentId,
        [FromServices] IQueryBus query,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = query.SendAsync<GetPaymentById, PaymentDetails>(
            GetPaymentById.Create(paymentId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: (result) => Results.Ok(result),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Request a new payment", OperationId = "request", Tags = new[] { "Payment" })]
    internal static async Task<IResult> Request(
        [FromBody] CreatePaymentRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var paymentId = Guid.NewGuid();
        var (orderId, amount) = request;
        var task = command.SendAsync(
            RequestPayment.Create(paymentId, orderId, amount), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Created(string.Empty, paymentId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Complete existing payment", OperationId = "complete", Tags = new[] { "Payment" })]
    internal static async Task<IResult> Complete(
        [FromRoute] Guid paymentId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(
            CompletePayment.Create(paymentId), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.NoContent(),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Discard existing payment", OperationId = "discard", Tags = new[] { "Payment" })]
    internal static async Task<IResult> Discard(
        [FromRoute] Guid paymentId,
        [FromBody] DiscardPaymentRequest request,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(
            DiscardPayment.Create(paymentId, request.Reason), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.Accepted(string.Empty, paymentId),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }

    [SwaggerOperation(Summary = "Timeout existing payment", OperationId = "timeout", Tags = new[] { "Payment" })]
    internal static async Task<IResult> Timeout(
        [FromRoute] Guid paymentId,
        [FromServices] ICommandBus command,
        [FromServices] ILoggerFactory logger,
        CancellationToken cancellationToken)
    {
        var log = logger.CreateLogger<Program>();
        var task = command.SendAsync(
            TimeOutPayment.Create(paymentId, DateTime.UtcNow), cancellationToken);

        return await WithCancellation.TryExecute(
            task: task,
            onCompleted: () => Results.NoContent(),
            onCancelled: (ex) => log.LogWarning(ex.Message),
            cancellationToken: cancellationToken
        );
    }
}