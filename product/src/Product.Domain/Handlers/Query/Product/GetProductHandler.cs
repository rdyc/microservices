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
    internal class GetProductHandler : IRequestHandler<GetProductQuery, IProductDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Product.GetAll()
                .Include(e => e.Currency)
                .Include(e => e.Attributes)
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id), cancellationToken);

            return mapper.Map<ProductDto>(result);
        }
    }
}