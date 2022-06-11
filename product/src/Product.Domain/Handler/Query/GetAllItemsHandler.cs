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
    internal class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, IEnumerable<IItemDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<IItemDto>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Item.GetAll().ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<ItemDto>>(result);
        }
    }
}