using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Payment.Payments.GettingPayments;

public record GetPayments(
    PagedOption Option
) : IQuery<IListPaged<PaymentShortInfo>>
{
    public static GetPayments Create(int? page, int? size) =>
        new(PagedOption.Create(page ?? 1, size ?? 10));
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
        var filter = Builders<PaymentShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, request.Option, cancellationToken);
    }
}