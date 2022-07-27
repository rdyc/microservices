using FluentValidation;
using MongoDB.Driver;
using Shipment.Orders.GettingOrders;

namespace Shipment.Orders;

public static class OrderValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistOrder<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IMongoCollection<Order> collection
    ) => ruleBuilder
        .MustAsync(async (value, cancellationToken) => await collection
            .CountDocumentsAsync(e => e.Id == value, null, cancellationToken) == 1)
        .WithMessage("The order was not found");
}