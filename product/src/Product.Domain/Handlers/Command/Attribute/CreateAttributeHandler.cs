using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Dtos;
using Product.Domain.Repositories;
using Product.Domain.Dtos;
using Product.Contract.Commands;

namespace Product.Domain.Handlers;

internal class CreateAttributeHandler : IRequestHandler<CreateAttributeCommand, IAttributeDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CreateAttributeHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IAttributeDto> Handle(CreateAttributeCommand request, CancellationToken cancellationToken)
    {
        var attribute = unitOfWork.Config.CreateAttribute(request.Name, request.Type, request.Unit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<AttributeDto>(attribute);
    }
}