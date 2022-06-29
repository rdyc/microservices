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

internal class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<IProductDto>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public GetAllProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<IProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Product.GetAll()
            .Include(e => e.Currency)
            .WithCriteria(request.Criteria)
            .WithOrdered(request.Ordered)
            .WithPaged(request.Paged)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<IProductDto>>(result);
    }
}