using System.Threading;
using System.Threading.Tasks;
using Core.Events;
using Product.Domain.Events;

namespace Product.Domain.Handlers.Event
{
    public class CurrencyUpdatedEventHandler :
        IEventHandler<CurrencyUpdatedEvent>,
        IEventHandler<EventEnvelope<CurrencyUpdatedEvent>>
    {
        public Task Handle(CurrencyUpdatedEvent @event, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task Handle(EventEnvelope<CurrencyUpdatedEvent> @event, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}