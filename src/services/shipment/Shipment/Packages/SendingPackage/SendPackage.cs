using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Shipment.Orders;
using Shipment.Orders.GettingOrders;
using Shipment.Products;

namespace Shipment.Packages.SendingPackage;

public record SendPackage(
    Guid PackageId,
    Guid OrderId,
    IEnumerable<PackageItem> Items,
    DateTime SentAt
) : ICommand
{
    public static SendPackage Create(
        Guid packageId,
        Guid orderId,
        IEnumerable<PackageItem> items,
        DateTime sentAt) =>
        new(packageId, orderId, items, sentAt);
}

public record PackageItem(
    Guid ProductId,
    int Quantity
);

internal class ValidateSendPackage : AbstractValidator<SendPackage>
{
    public ValidateSendPackage(IMongoDatabase database)
    {
        var orders = database.GetCollection<Order>(MongoHelper.GetCollectionName<Order>());
        var products = database.GetCollection<Product>(MongoHelper.GetCollectionName<Product>());

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PackageId).NotEmpty();

        RuleFor(p => p.OrderId).NotEmpty()
            .MustExistOrder(orders);

        RuleForEach(p => p.Items).NotEmpty()
            .ChildRules(item => 
            {
                item.RuleFor(p => p.ProductId).NotEmpty()
                        .MustExistProduct(products);

                item.RuleFor(p => p.Quantity).NotEmpty()
                        .MustInStockProduct(products);
            });

        RuleFor(p => p.SentAt).NotEmpty();
    }
}

internal class HandleSendPackage : ICommandHandler<SendPackage>
{
    private readonly IEventStoreDBRepository<Package> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleSendPackage(
        IEventStoreDBRepository<Package> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(SendPackage request, CancellationToken cancellationToken)
    {
        var (packageId, _, items, sentAt) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                packageId,
                package => package.Sent(items, sentAt),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}