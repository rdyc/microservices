using FW.Core.Aggregates;

namespace Cart.ShoppingCarts;

public class ShoppingCart : Aggregate
{
    public override void When(object @event)
    {
        base.When(@event);
    }
}