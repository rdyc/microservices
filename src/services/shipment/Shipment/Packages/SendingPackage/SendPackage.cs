using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Shipment.Orders;
using Shipment.Orders.GettingOrders;

namespace Shipment.Packages.SendingPackage;

public record SendPackage(
    Guid PackageId,
    Guid OrderId,
    DateTime SentAt
) : ICommand
{
    public static SendPackage Create(Guid packageId, Guid orderId, DateTime sentAt) =>
        new(packageId, orderId, sentAt);
}

internal class ValidateSendPackage : AbstractValidator<SendPackage>
{
    public ValidateSendPackage(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Order>();
        var collection = database.GetCollection<Order>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PackageId).NotEmpty();

        RuleFor(p => p.OrderId).NotEmpty()
            .MustExistOrder(collection);

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
        var (packageId, _, sentAt) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                packageId,
                package => package.Sent(sentAt),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}