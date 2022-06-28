using System;
using Core.Aggregates;
using Product.Domain.Events;

namespace Product.Domain.Models;

public class CurrencyModel : Aggregate
{
    private CurrencyModel() { }

    public string Name { get; private set; }
    public string Code { get; private set; }
    public string Symbol { get; private set; }

    internal static CurrencyModel Create(string name, string code, string symbol)
    {
        return new CurrencyModel(Guid.NewGuid(), name, code, symbol);
    }

    public void Update(Guid id, string name, string code, string symbol)
    {
        var @event = new CurrencyUpdatedEvent(id, name, code, symbol);

        Enqueue(@event);
        Apply(@event);
    }

    private CurrencyModel(Guid id, string name, string code, string symbol)
    {
        var @event = new CurrencyCreatedEvent(id, name, code, symbol);

        Enqueue(@event);
        Apply(@event);
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case CurrencyCreatedEvent evt:
                Apply(evt);
                return;
            case CurrencyUpdatedEvent evt:
                Apply(evt);
                return;
            case CurrencyRemovedEvent evt:
                Apply(evt);
                return;
        }
    }

    private void Apply(CurrencyCreatedEvent @event)
    {
        Version = 0;

        Id = @event.Id;
        Name = @event.Name;
        Code = @event.Code;
        Symbol = @event.Symbol;
    }

    private void Apply(CurrencyUpdatedEvent @event)
    {
        Version++;

        Name = @event.Name;
        Code = @event.Code;
        Symbol = @event.Symbol;
    }

    private void Apply(CurrencyRemovedEvent _)
    {
        Version++;
    }
}