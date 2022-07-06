using FW.Core.Aggregates;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;

namespace Lookup.Attributes;

public class Attribute : Aggregate
{
    public string Name { get; private set; } = default!;
    public AttributeType Type { get; private set; } = default!;
    public string Unit { get; private set; } = default!;
    public LookupStatus Status { get; private set; } = default!;

    public static Attribute Register(Guid? id, string name, AttributeType type, string unit, LookupStatus status)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));

        return new Attribute(id.Value, name, type, unit, status);
    }

    private Attribute() { }

    private Attribute(Guid id, string name, AttributeType code, string unit, LookupStatus status)
    {
        var evt = AttributeRegistered.Create(id, name, code, unit, status);

        Enqueue(evt);
        Apply(evt);
    }

    public override void When(object evt)
    {
        switch (evt)
        {
            case AttributeRegistered registered:
                Apply(registered);
                return;
            case AttributeModified modified:
                Apply(modified);
                return;
            case AttributeRemoved removed:
                Apply(removed);
                return;
        }
    }

    public void Apply(AttributeRegistered evt)
    {
        Id = evt.Id;
        Name = evt.Name;
        Type = evt.Type;
        Unit = evt.Unit;
        Status = LookupStatus.Active;
    }

    public void Modify(string name, AttributeType type, string unit)
    {
        if (Status == LookupStatus.Removed)
            throw new InvalidOperationException($"The attribute was removed");

        var evt = AttributeModified.Create(Id, name, type, unit);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(AttributeModified evt)
    {
        Version++;

        Name = evt.Name;
        Type = evt.Type;
        Unit = evt.Unit;
    }

    public void Remove()
    {
        if (Status == LookupStatus.Removed)
            throw new InvalidOperationException($"The attribute already removed");

        var evt = AttributeRemoved.Create(Id);

        Enqueue(evt);
        Apply(evt);
    }

    public void Apply(AttributeRemoved _)
    {
        Version++;

        Status = LookupStatus.Removed;
    }
}