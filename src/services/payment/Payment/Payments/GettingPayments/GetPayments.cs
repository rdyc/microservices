using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace  Payment.Payments.GettingPayments;

public record GetPayments(
    int Index,
    int Size
) : IQuery<IListPaged<PaymentShortInfo>>
{
    public static GetPayments Create(int? index = 0, int? size = 10)
    {
        if (index is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (size is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(size));

        return new(index.Value, size.Value);
    }
}

internal class HandleGetPayments : IQueryHandler<GetPayments, IListPaged<PaymentShortInfo>>
{
    private readonly IMongoCollection<PaymentShortInfo> collection;

    public HandleGetPayments(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<PaymentShortInfo>();
        this.collection = database.GetCollection<PaymentShortInfo>(collectionName);
    }

    public async Task<IListPaged<PaymentShortInfo>> Handle(GetPayments request, CancellationToken cancellationToken)
    {
       var (index, size) = request;

        var filter = Builders<PaymentShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}