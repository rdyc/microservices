using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Dtos;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers;

internal class GetAttributeHandler : IRequestHandler<GetAttributeQuery, IAttributeDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public GetAttributeHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IAttributeDto> Handle(GetAttributeQuery request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Config.GetAllAttributes()
            .SingleOrDefaultAsync(e => e.Id.Equals(request.Id), cancellationToken);

        return mapper.Map<AttributeDto>(result);
    }
}