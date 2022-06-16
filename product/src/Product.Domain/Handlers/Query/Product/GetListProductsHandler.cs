using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Dtos;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers
{
    internal class GetListProductsHandler : IRequestHandler<GetListProductsQuery, IEnumerable<IProductDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetListProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<IProductDto>> Handle(GetListProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Product.GetAll()
                .WithCriteria(request.Criteria)
                .WithOrdered(request.Ordered)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<ProductDto>>(result);
        }
    }
}