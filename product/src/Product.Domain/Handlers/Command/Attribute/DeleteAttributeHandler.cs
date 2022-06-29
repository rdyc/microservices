using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Domain.Repositories;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Domain.Dtos;

namespace Product.Domain.Handlers;

internal class DeleteAttributeHandler : IRequestHandler<DeleteAttributeCommand, IAttributeDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public DeleteAttributeHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IAttributeDto> Handle(DeleteAttributeCommand request, CancellationToken cancellationToken)
    {
        // get existing attribute
        var attribute = await unitOfWork.Config.GetAttributeAsync(request.Id.Value, cancellationToken);

        // set attribute modification
        unitOfWork.Config.UpdateAttribute(attribute, p => p.IsDeleted = true);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<AttributeDto>(attribute);
    }
}