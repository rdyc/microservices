using FW.Core.Aggregates;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;

namespace Lookup.Currencies;

public class Currency : Aggregate
{
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string Symbol { get; private set; } = default!;
    public LookupStatus Status { get; private set; } = default!;

    public static Currency Register(Guid? id, string name, string code, string symbol, LookupStatus status)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));

        return new Currency(id.Value, name, code, symbol, status);
    }

    private Currency() { }

    public override void When(object evt)
    {
        switch (evt)
        {
            case CurrencyRegistered currency:
                Apply(currency);
                return;
            case CurrencyModified currency:
                Apply(currency);
                return;
            case CurrencyRemoved currency:
                Apply(currency);
                return;
        }
    }

    private Currency(Guid id, string name, string code, string symbol, LookupStatus status)
    {
        var evt = CurrencyRegistered.Create(id, name, code, symbol, status);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(CurrencyRegistered evt)
    {
        Id = evt.Id;
        Name = evt.Name;
        Code = evt.Code;
        Symbol = evt.Symbol;
        Status = LookupStatus.Active;
    }

    public void Modify(string name, string code, string symbol)
    {
        if (Status == LookupStatus.Removed)
            throw new InvalidOperationException($"The currency was removed");

        var evt = CurrencyModified.Create(Id, name, code, symbol);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(CurrencyModified evt)
    {
        Version++;

        Name = evt.Name;
        Code = evt.Code;
        Symbol = evt.Symbol;
    }

    public void Remove()
    {
        if (Status == LookupStatus.Removed)
            throw new InvalidOperationException($"The currency already removed");

        var evt = CurrencyRemoved.Create(Id);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(CurrencyRemoved _)
    {
        Version++;

        Status = LookupStatus.Removed;
    }
}