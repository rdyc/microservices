using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Order.Orders.GettingOrderById;

public record GetOrderById(
    Guid ClientId,
    Guid OrderId
) : IQuery<OrderDetails>
{
    public static GetOrderById Create(Guid clientId, Guid orderId) =>
        new(clientId, orderId);
}

internal class HandleGetOrderById : IQueryHandler<GetOrderById, OrderDetails>
{
    private readonly IMongoCollection<OrderDetails> collection;

    public HandleGetOrderById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<OrderDetails>();
        this.collection = database.GetCollection<OrderDetails>(collectionName);
    }

    public async Task<OrderDetails> Handle(GetOrderById request, CancellationToken cancellationToken)
    {
        var (clientId, orderId) = request;
        var result = await collection
            .Find(e => e.ClientId.Equals(clientId) && e.Id.Equals(orderId))
            .SingleOrDefaultAsync(cancellationToken);

        if (result is null)
            throw AggregateNotFoundException.For<OrderDetails>(orderId);

        return result;
    }
}