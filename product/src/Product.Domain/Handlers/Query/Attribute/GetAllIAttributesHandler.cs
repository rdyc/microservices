using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers;

internal class GetAllAttributesHandler : IRequestHandler<GetAllAttributesQuery, IEnumerable<IAttributeDto>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public GetAllAttributesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<IAttributeDto>> Handle(GetAllAttributesQuery request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Config.GetAllAttributes()
            .WithCriteria(request.Criteria)
            .WithOrdered(request.Ordered)
            .WithPaged(request.Paged)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<IAttributeDto>>(result);
    }
}