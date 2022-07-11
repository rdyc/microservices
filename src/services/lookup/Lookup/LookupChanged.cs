using FW.Core.Events;

namespace Lookup;

public record LookupChanged : IExternalEvent
{
    private LookupChanged(Context context, State state, object data)
    {
        Context = context;
        State = state;
        Data = data;
    }

    public static LookupChanged Create(Context context, State state, object data)
        => new(context, state, data);

    public Context Context { get; }
    public State State { get; }
    public object Data { get; }
}

public enum Context
{
    Attribute, Currency
}

public enum State
{
    Added, Updated, Deleted
}