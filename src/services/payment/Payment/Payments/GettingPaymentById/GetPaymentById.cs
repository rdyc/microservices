using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using MongoDB.Driver;

namespace  Payment.Payments.GettingPaymentById;

public record GetPaymentById(
    Guid PaymentId
) : IQuery<PaymentDetails>
{
    public static GetPaymentById Create(Guid paymentId) => new(paymentId);
}

internal class HandleGetPaymentById : IQueryHandler<GetPaymentById, PaymentDetails>
{
    private readonly IMongoCollection<PaymentDetails> collection;

    public HandleGetPaymentById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentDetails>();
        this.collection = database.GetCollection<PaymentDetails>(collectionName);
    }

    public async Task<PaymentDetails> Handle(GetPaymentById request, CancellationToken cancellationToken)
    {
        var result = await collection.Find(e => e.Id.Equals(request.PaymentId))
            .SingleOrDefaultAsync(cancellationToken);

        if (result is null)
            throw AggregateNotFoundException.For<Payment>(request.PaymentId);

        return result;
    }
}