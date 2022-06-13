using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dto;
using Product.Contract.Query;
using Product.Domain.Repository;
using Product.Domain.Dto;

namespace Product.Domain.Handler.Query
{
    internal class GetListItemsHandler : IRequestHandler<GetListItemsQuery, IEnumerable<IItemDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetListItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<IItemDto>> Handle(GetListItemsQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.ProductItem.GetAll()
                .WithCriteria(request.Criteria)
                .WithOrdered(request.Ordered)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<ItemDto>>(result);
        }
    }
}