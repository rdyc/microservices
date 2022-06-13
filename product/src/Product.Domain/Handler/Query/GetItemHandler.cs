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
    internal class GetItemHandler : IRequestHandler<GetItemQuery, IItemDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IItemDto> Handle(GetItemQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.ProductItem.GetAll()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id), cancellationToken);

            return mapper.Map<ItemDto>(result);
        }
    }
}