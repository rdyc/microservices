using Lookup.Currencies.RegisterCurrency;
using FW.Core.Aggregates;

namespace Lookup.Currencies;

public class Currency : Aggregate
{
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string Symbol { get; private set; } = default!;
    public CurrencyStatus Status { get; private set; } = default!;

    public static Currency Register(Guid id, string name, string code, string symbol)
    {
        return new Currency(id, name, code, symbol);
    }

    private Currency() { }

    public override void When(object @event)
    {
        switch (@event)
        {
            case CurrencyRegistered currency:
                Apply(currency);
                return;
                /* case ProductAdded cartOpened:
                    Apply(cartOpened);
                    return;
                case ProductRemoved cartOpened:
                    Apply(cartOpened);
                    return;
                case ShoppingCartConfirmed cartOpened:
                    Apply(cartOpened);
                    return;
                case ShoppingCartCanceled cartCanceled:
                    Apply(cartCanceled);
                    return; */
        }
    }

    private Currency(Guid id, string name, string code, string symbol)
    {
        var evt = CurrencyRegistered.Create(id, name, code, symbol);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(CurrencyRegistered @event)
    {
        Id = @event.Id;
        Name = @event.Name;
        Code = @event.Code;
        Symbol = @event.Symbol;
        Status = CurrencyStatus.Active;
    }


    /* public void Confirm()
    {
        if (Status != ShoppingCartStatus.Pending)
            throw new InvalidOperationException($"Confirming cart in '{Status}' status is not allowed.");

        var @event = ShoppingCartConfirmed.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ShoppingCartConfirmed @event)
    {
        Status = ShoppingCartStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != ShoppingCartStatus.Pending)
            throw new InvalidOperationException($"Canceling cart in '{Status}' status is not allowed.");

        var @event = ShoppingCartCanceled.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ShoppingCartCanceled @event)
    {
        Status = ShoppingCartStatus.Canceled;
    } */
}
