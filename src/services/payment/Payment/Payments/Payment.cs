using FW.Core.Aggregates;
using Payment.Payments.CompletingPayment;
using Payment.Payments.DiscardingPayment;
using Payment.Payments.RequestingPayment;
using Payment.Payments.TimingOutPayment;

namespace Payment.Payments;

public class Payment : Aggregate
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }

    public Payment() { }

    private Payment(Guid id, Guid orderId, decimal amount)
    {
        var @event = PaymentRequested.Create(id, orderId, amount, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case PaymentRequested requested:
                Apply(requested);
                return;
            case PaymentCompleted completed:
                Apply(completed);
                return;
            case PaymentDiscarded discarded:
                Apply(discarded);
                return;
            case PaymentTimedOut expired:
                Apply(expired);
                return;
        }
    }

    public static Payment Initialize(Guid paymentId, Guid orderId, decimal amount)
        => new(paymentId, orderId, amount);

    public void Apply(PaymentRequested @event)
    {
        Id = @event.PaymentId;
        OrderId = @event.OrderId;
        Amount = @event.Amount;
        RequestedAt = @event.RequestedAt;
        Version = 0;
    }

    public void Complete()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Completing payment in '{Status}' status is not allowed.");

        var @event = PaymentCompleted.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PaymentCompleted _)
    {
        Version++;

        Status = PaymentStatus.Completed;
    }

    public void Discard(DiscardReason discardReason)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Discarding payment in '{Status}' status is not allowed.");

        var @event = PaymentDiscarded.Create(Id, discardReason, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PaymentDiscarded _)
    {
        Version++;

        Status = PaymentStatus.Discarded;
    }

    public void TimeOut()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Discarding payment in '{Status}' status is not allowed.");

        var @event = PaymentTimedOut.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PaymentTimedOut _)
    {
        Version++;

        Status = PaymentStatus.Failed;
    }
}